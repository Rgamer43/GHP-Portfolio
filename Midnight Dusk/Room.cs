//using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static System.Math;

public class Room : MonoBehaviour
{

    public GameObject openEntranceV;
    public GameObject closedEntranceV;
    public GameObject openEntranceVNorth;
    public GameObject closedEntranceVNorth;

    public GameObject openEntranceH;
    public GameObject closedEntranceH;

    public Transform[] exitSpots = new Transform[4];
    public bool[] exits = new bool[4] { false, false, false, false };

    public Vector2 pos, origin, scale;

    [SerializeField] public bool[][] moveGrid; //Tracks which tiles are passable and which are not
    public bool[][] coverGrid; //Tracks which tiles have cover from the player

    public int height, width;

    public int[] encounterPowerRange;
    public List<Enemy> enemies;

    public bool isPlayerInRoom = false;

    public GameObject player;
    public Player playerScript;

    public int intervalCount = 0;

    public List<Door> doors = new List<Door>();

    public bool spawnedEnemies = false;

    public static Room roomToTick;
    public static bool tickThreadRunning = false;
    public bool isRunning = true;

    // Start is called before the first frame update
    void Start()
    {
        if(!tickThreadRunning)
        {
            tickThreadRunning = true;
            Thread thread = new Thread(ThreadedTick);
            thread.Start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");
            playerScript = player.GetComponent<Player>();
        }

        Vector2 p = WorldToRoomPos(player.transform.position, this);
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) isPlayerInRoom = false;
        else isPlayerInRoom = true;
        Log.LogMsg("Player Pos: " + p + ", " + isPlayerInRoom);

        isRunning = Application.IsPlaying(this);
    }

    IEnumerator IntervalLoop()
    {
        SpawnEnemies();
        Log.LogMsg("Started interval coroutine for room " + pos.x + ", " + pos.y);
        Log.LogMsg("Staggering interval by " + (pos.x + (pos.y / 10)) + " secs");
        yield return new WaitForSeconds(pos.x + (pos.y / 10));
        while (true)
        {
            StartCoroutine(Interval());
            if (isPlayerInRoom) yield return new WaitForSecondsRealtime(Options.OCCUPIED_ROOM_TICK_RATE);
            else yield return new WaitForSecondsRealtime(Options.UNOCCUPIED_ROOM_TICK_RATE);
        }
    }

    IEnumerator Interval()
    {
        intervalCount++;

        Vector2 pPos = WorldToRoomPos(new Vector2(player.transform.position.x, player.transform.position.y), this);
        //if (pPos.x >= 0 && pPos.x < width && pPos.y >= 0 && pPos.y < height) isPlayerInRoom = true;
        //else isPlayerInRoom = false;

        //Log.LogMsg("IsPlayerInRoom for " + pos.x + ", " + pos.y + ": " + isPlayerInRoom);
        //Log.LogMsg("pPos: " + pPos.x + ", " + pPos.y);

        if (enemies.Count > 0 && isPlayerInRoom) foreach (Door i in doors) {
                try
                {
                    if (i != null)
                    {
                        i.locked = true; i.EnableCollider();
                    }
                } catch { }
            }
        else { foreach (Door i2 in doors)
            {
                try
                {
                    if (i2 != null) i2.locked = false;
                } catch { }
            }
        }

        if (isPlayerInRoom)
        {
            Log.LogMsg("Starting interval " + intervalCount + " for room " + pos.x + ", " + pos.y);
            Log.LogMsg("PPos: " + pPos.x + ", " + pPos.y);

            roomToTick = this;

            GenGrids();

            string s = "";
            for(int i = 0; i < moveGrid[0].Length; i++)
            {
                if (s != "") s += "\n";
                for (int j = 0; j < moveGrid.Length; j++)
                {
                    if (moveGrid[j][i]) s += "T";
                    else s += "F";
                }
            }
            Log.LogMsg("MoveGrid: \n" + s);

            s = "";
            for (int i = 0; i < coverGrid[0].Length; i++)
            {
                if (s != "") s += "\n";
                for (int j = 0; j < coverGrid.Length; j++)
                {
                    if (coverGrid[j][i]) s += "T";
                    else s += "F";
                }
            }
            Log.LogMsg("CoverGrid: \n" + s);

                if (!spawnedEnemies) SpawnEnemies();

            for (int i = 0; i < enemies.Count; i++)
            {
                try
                {
                    
                } catch (System.Exception e)
                {
                    Log.LogException(e);
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public static void ThreadedTick()
    {
        Log.LogMsg("Started tick thread");
        while(true)
        {
            if (roomToTick != null)
            {
                Room room = roomToTick;
                if (room.isPlayerInRoom)
                {
                    Debug.Log("Started threaded tick " + room.intervalCount + " for room " + room.pos);

                    for (int i = 0; i < room.enemies.Count; i++)
                    {
                        try
                        {
                            Enemy e = room.enemies[i];
                            int attempts = 0;
                            Log.LogMsg("Doing interval for enemy " + i + "...");
                            Vector2 et;
                            if (e.path.Count > 0) et = WorldToRoomPos(new Vector2(e.path[e.path.Count - 1].x,
                                 e.path[e.path.Count - 1].y), room);
                            else et = new Vector2(0, 0);
                            Log.LogMsg("Enemy Target: " + et.x + ", " + et.y);

                            //Log.LogMsg("Enemy Ammo: " + e.weapon.ammo);
                            if (e.weapon.ammo > 0)
                            {
                                Log.LogMsg("Enemy has ammo.");

                                bool pf = e.shouldPathfind;
                                //try
                                //{
                                if (e.path.Count == 0)
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF.");
                                }
                                else if (e.path.Count - 1 < 0 || e.path.Count - 1 >= e.path.Count)
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF.");

                                }
                                else if (et.x < 0 || et.x > room.coverGrid.Length || et.y < 0 || et.y >= room.coverGrid[0].Length)
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF.");
                                }
                                else if (room.coverGrid[(int)et.x]
                                [(int)et.y])
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF.");
                                }
                                else if (e.path.Count == 0 || RandomRange(0, 100) < -1 || room.intervalCount == 1)
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF.");
                                }
                                else if (e.shouldPathfind)
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF. Setting other stuff...");
                                    e.shouldPathfind = false;
                                    e.timeSinceReachedPoint = 0f;
                                    e.path = new List<Vector2>();
                                    Log.LogMsg("Set other stuff.");
                                }

                                if (e.pathfinding) pf = false;
                                //} catch (System.Exception exc)
                                //{
                                //    pf = true;
                                //    Log.LogError("Caught error in checking if should pathfind.");
                                //    Log.LogError(exc.ToString());
                                //}

                                Log.LogMsg("PF: " + pf);

                                if (pf)
                                {
                                    Log.LogMsg("Pathfinding");
                                    Vector2 target = new Vector2(RandomRange(0, room.width), RandomRange(0, room.height));
                                    //while (!room.moveGrid[(int)target.x][(int)target.y] || (attempts < Options.ATTEMPTS_LIMIT_REALTIME && room.coverGrid[(int)target.x][(int)target.y] &&
                                    //    Vector2.Distance(WorldToRoomPos(room.playerScript.position, room), target) > e.weapon.range[0] * 0.2 &&
                                    //    Vector2.Distance(WorldToRoomPos(room.playerScript.position, room), target) <= e.weapon.range[1] * 1.1))
                                    //{
                                    //    target = new Vector2(RandomRange(0, room.width), RandomRange(0, room.height));
                                    //    attempts++;
                                    //
                                    //}
                                    target = FindTarget(WorldToRoomPos(e.position, room), false, room.moveGrid, room.coverGrid);

                                    Vector2 pfPos = WorldToRoomPos(room.enemies[i].position, room);

                                    Log.LogMsg("Starting pathfind thread...");
                                    Thread thread = new Thread(Pathfinding.Pathfind);
                                    thread.Start(new object[] {
                                room.moveGrid, pfPos, target, room, room.enemies[i]
                            });
                                    //StartCoroutine(Pathfinding.Pathfind(moveGrid, WorldToRoomPos(enemies[i].position, this), target, this, enemies[i]));
                                }
                                else Log.LogMsg("Not pathfinding");

                                Log.LogMsg("Attempts to find pathfind target: " + attempts);
                            }
                            else
                            {
                                Log.LogMsg("Enemy does not have ammo.");
                                Vector2 ent = new Vector2(-1, -1);
                                if (e.path.Count > 0) ent = WorldToRoomPos(new Vector2(e.path[e.path.Count - 1].x, e.path[e.path.Count - 1].y), room);
                                Log.LogMsg("Enemy Target: " + ent.x + ", " + ent.y);

                                bool pf = e.shouldPathfind;
                                if (e.path.Count == 0)
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF.");
                                }
                                else if (!room.coverGrid[(int)ent.x][(int)ent.y])
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF.");
                                }
                                else if (e.path.Count == 0 || RandomRange(0, 100) < 3 || room.intervalCount == 1)
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF.");
                                }
                                else if (e.shouldPathfind)
                                {
                                    Log.LogMsg("Setting PF...");
                                    pf = true;
                                    Log.LogMsg("Set PF. Setting other stuff...");
                                    e.shouldPathfind = false;
                                    e.timeSinceReachedPoint = 0f;
                                    e.path = new List<Vector2>();
                                    Log.LogMsg("Set other stuff.");
                                }

                                if (e.pathfinding) pf = false;

                                if (pf)
                                {
                                    Log.LogMsg("Pathfinding");
                                    Vector2 target = new Vector2(RandomRange(0, room.width), RandomRange(0, room.height));
                                    //while (!room.moveGrid[(int)target.x][(int)target.y] || (attempts < Options.ATTEMPTS_LIMIT_REALTIME && !room.coverGrid[(int)target.x][(int)target.y]))
                                    //{
                                    //    target = new Vector2(RandomRange(0, room.width), RandomRange(0, room.height));
                                    //    attempts++;
                                    //}
                                    target = FindTarget(WorldToRoomPos(e.position, room), false, room.moveGrid, room.coverGrid);

                                    Vector2 pfPos = WorldToRoomPos(room.enemies[i].position, room);

                                    Log.LogMsg("Starting pathfind thread...");
                                    Thread thread = new Thread(Pathfinding.Pathfind);
                                    thread.Start(new object[] {
                                room.moveGrid, pfPos, target, room, room.enemies[i]
                            });
                                    //StartCoroutine(Pathfinding.Pathfind(moveGrid, WorldToRoomPos(enemies[i].position, this), target, this, enemies[i]));
                                }
                                else Log.LogMsg("Not pathfinding");
                            }

                            Log.LogMsg("Finished interval for enemy " + i + ".");
                        }
                        catch (System.Exception e)
                        {
                            Log.LogException(e);
                        }
                    }
                    Log.LogMsg("Finished tick");
                    Thread.Sleep(50);
                }
            }
        }
    }

    public static Vector2 FindTarget(Vector2 start, bool coverTarget, bool[][] moveGrid, bool[][] coverGrid)
    {
        int searchDist = 1;
        List<Vector2> dirs = new List<Vector2>();
        Vector2 current = start;
        int attempts = 0;

        while(attempts < Options.ATTEMPTS_LIMIT_REALTIME)
        {
            dirs = new List<Vector2>();
            for(int x = -searchDist; x < searchDist + 1; x++)
            {
                Vector2 c = new Vector2(x + start.x, -searchDist + start.y);
                if (IsTileValid(c, moveGrid))
                    if (GetMoveable(c, moveGrid) && GetCovered(c, coverGrid) == coverTarget)
                        return c;

                c = new Vector2(x + start.x, searchDist + start.y);
                if (IsTileValid(c, moveGrid))
                    if (GetMoveable(c, moveGrid) && GetCovered(c, coverGrid) == coverTarget)
                        return c;

                c = new Vector2(searchDist + start.x, x + start.y);
                if (IsTileValid(c, moveGrid))
                    if (GetMoveable(c, moveGrid) && GetCovered(c, coverGrid) == coverTarget)
                        return c;

                c = new Vector2(-searchDist + start.x, x + start.y);
                if (IsTileValid(c, moveGrid))
                    if (GetMoveable(c, moveGrid) && GetCovered(c, coverGrid) == coverTarget)
                        return c;
            }
            attempts++;
            searchDist++;
        }

        return start;
    }


    public static bool GetMoveable(Vector2 pos, bool[][] grid)
    {
        return grid[(int)pos.x][(int)pos.y];
    }

    public static bool GetCovered(Vector2 pos, bool[][] grid)
    {
        return grid[(int)pos.x][(int)pos.y];
    }

    public static bool IsTileValid(Vector2 pos, bool[][] grid)
    {
        if (pos.x < 0 || pos.x >= grid.Length) return false;
        else if (pos.y < 0 || pos.y > grid[(int)pos.x].Length) return false;
        return true;
    }

    public static int RandomRange(int min, int max)
    {
        return new System.Random().Next(max - min) + min;
    }

    public void Init()
    {
        Log.LogMsg("Initting room at  " + pos.x + ", " + pos.y);
        Log.LogMsg("Adding closed exits for " + pos.x + ", " + pos.y);
        for (int i = 0; i < exits.Length; i++)
        {
            Log.LogMsg("Index " + i + " is " + exits[i]);
            if (!exits[i])
            {
                Log.LogMsg("Adding closed exit at location " + i);
                if (i == 2)
                {
                    GameObject e = Instantiate(closedEntranceV, exitSpots[2].position, Quaternion.identity);
                    e.transform.localScale = new Vector3(1 * e.transform.localScale.x, 1 * e.transform.localScale.y, 1);
                }
                else if (i == 0)
                {
                    GameObject e = Instantiate(closedEntranceV, exitSpots[0].position, Quaternion.identity);
                }
                else if (i == 1)
                {
                    GameObject e = Instantiate(closedEntranceH, exitSpots[1].position, Quaternion.identity);
                }
                else if (i == 3)
                {
                    GameObject e = Instantiate(closedEntranceH, exitSpots[3].position, Quaternion.identity);
                    e.transform.localScale = new Vector3(-1 * e.transform.localScale.x, 1 * e.transform.localScale.y, 1);
                }
            }
        }

        scale = new Vector2(GameObject.Find("Room " + pos.x + " " + pos.y).transform.lossyScale.x, GameObject.Find("Room " + pos.x + " " + pos.y).transform.lossyScale.y);
        Log.LogMsg("Scale: " + scale.x + ", " + scale.y);

        Log.LogMsg("Starting initialization part 2 coroutine...");
        StartCoroutine(Init2());
    }

    IEnumerator Init2()
    {
        Log.LogMsg("Started coroutine");
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Log.LogMsg("Secondarily initting room at  " + pos.x + ", " + pos.y);

        moveGrid = new bool[width][];
        coverGrid = new bool[width][];
        for (int i = 0; i < moveGrid.Length; i++)
        {
            moveGrid[i] = new bool[height];
            coverGrid[i] = new bool[height];
        }

        Log.LogMsg("Initting Grid...");
        for (int x = 0; x < moveGrid.Length; x++)
            for (int y = 0; y < moveGrid[x].Length; y++) moveGrid[x][y] = true;

        Log.LogImportant("MoveGrid Size: ");
        Log.LogMsg(moveGrid.Length.ToString());
        Log.LogMsg(moveGrid[0].Length.ToString());

        for (int x = 0; x < moveGrid.Length; x++) for(int y = 0; y < moveGrid[x].Length; y++)
            {
                moveGrid[x][y] = true;
                float rayDist = Options.ROOM_GRID_RAY_DIST;

                RaycastHit2D hit = Physics2D.Raycast(RoomToWorldPos(new Vector2(x, y), this), new Vector2(1, 1), rayDist);
                if (hit.collider != null)
                {
                    moveGrid[x][y] = false;
                    Debug.DrawRay(RoomToWorldPos(new Vector2(x, y), this), new Vector2(rayDist, rayDist), Color.red, 10);
                }
                else Debug.DrawRay(RoomToWorldPos(new Vector2(x, y), this), new Vector2(rayDist, rayDist), Color.yellow, 10);

                hit = Physics2D.Raycast(RoomToWorldPos(new Vector2(x, y), this), new Vector2(-1, 1), rayDist);
                if (hit.collider != null)
                {
                    moveGrid[x][y] = false;
                    Debug.DrawRay(RoomToWorldPos(new Vector2(x, y), this), new Vector2(-rayDist, rayDist), Color.red, 10);
                }
                else Debug.DrawRay(RoomToWorldPos(new Vector2(x, y), this), new Vector2(-rayDist, rayDist), Color.yellow, 10);

                hit = Physics2D.Raycast(RoomToWorldPos(new Vector2(x, y), this), new Vector2(-1, -1), rayDist);
                if (hit.collider != null)
                {
                    moveGrid[x][y] = false;
                    Debug.DrawRay(RoomToWorldPos(new Vector2(x, y), this), new Vector2(-rayDist, -rayDist), Color.red, 10);
                }
                else Debug.DrawRay(RoomToWorldPos(new Vector2(x, y), this), new Vector2(-rayDist, -rayDist), Color.yellow, 10);

                hit = Physics2D.Raycast(RoomToWorldPos(new Vector2(x, y), this), new Vector2(1, -1), rayDist);
                if (hit.collider != null)
                {
                    moveGrid[x][y] = false;
                    Debug.DrawRay(RoomToWorldPos(new Vector2(x, y), this), new Vector2(rayDist, -rayDist), Color.red, 10);
                }
                else Debug.DrawRay(RoomToWorldPos(new Vector2(x, y), this), new Vector2(rayDist, -rayDist), Color.yellow, 10);
            }

        Log.LogMsg("Finished initting grid. Logging grid state...");
        for (int y = 0; y < moveGrid[0].Length; y++)
        {
            string m = "";
            for (int x = 0; x < moveGrid.Length; x++)
            {
                if (moveGrid[x][y]) m += "T";
                else m += "F";
            }
            Log.LogMsg(m);
        }
        Log.LogMsg("Finished logging grid state");

        StartCoroutine(IntervalLoop());
    }

    public void SpawnEnemies()
    {
        spawnedEnemies = true;

        double p = Random.Range((float)encounterPowerRange[0], (float)encounterPowerRange[1]);
        List<GameObject> en = DungeonGenerator.instance.faction.enemies;

        double i = 0;
        while(i < p)
        {
            Vector2 spawn = new Vector2(Random.Range(0, width), Random.Range(0, height));
            while(!moveGrid[(int)spawn.x][(int)spawn.y])
                spawn = new Vector2(Random.Range(0, width), Random.Range(0, height));

            spawn = RoomToWorldPos(spawn, this);
            int ind = Random.Range(0, en.Count);

            int spawning = Random.Range(DungeonGenerator.instance.faction.minSpawning[ind], DungeonGenerator.instance.faction.maxSpawning[ind] + 1);
            if (spawning == 0) spawning = 1;
            Log.LogMsg("Spawning enemy " + ind + " x" + spawning);

            for (int x = 0; x < spawning; x++)
            {
                spawn = new Vector2(Random.Range(0, width), Random.Range(0, height));
                while (!moveGrid[(int)spawn.x][(int)spawn.y])
                    spawn = new Vector2(Random.Range(0, width), Random.Range(0, height));
                spawn = RoomToWorldPos(spawn, this);

                GameObject se = Instantiate(en[ind], new Vector3(spawn.x, spawn.y, 0), Quaternion.identity);
                se.GetComponent<Enemy>().room = this;
                enemies.Add(se.GetComponent<Enemy>());
                se.GetComponent<Enemy>().Init();
            }
            i += DungeonGenerator.instance.faction.enemyPowers[ind];
        }
    }

    public void SetPos(Vector2 p)
    {
        pos = p;
    }

    public void AddEntrance(Vector2 endpoint)
    {
        int index = EndPointToIndex(endpoint);
        Log.LogMsg("Direction to Endpoint: " + index);
        if(index == 2)
        {
            GameObject e = Instantiate(openEntranceV, exitSpots[2].position, Quaternion.identity);
            exits[2] = true;
            e.transform.localScale = new Vector3(1 * e.transform.localScale.x, -1 * e.transform.localScale.y, 1);
            doors.Add(e.transform.GetChild(0).GetComponent<Door>());
            if(doors[doors.Count-1] == null) doors.Add(e.transform.GetChild(1).GetComponent<Door>());
        }
        else if(index == 0)
        {
            GameObject e = Instantiate(openEntranceVNorth, exitSpots[0].position, Quaternion.identity);
            exits[0] = true;
            doors.Add(e.transform.GetChild(0).GetComponent<Door>());
            if (doors[doors.Count - 1] == null) doors.Add(e.transform.GetChild(1).GetComponent<Door>());
        }
        else if (index == 1)
        {
            GameObject e = Instantiate(openEntranceH, exitSpots[1].position, Quaternion.identity);
            exits[1] = true;
            doors.Add(e.transform.GetChild(0).GetComponent<Door>());
            if (doors[doors.Count - 1] == null) doors.Add(e.transform.GetChild(1).GetComponent<Door>());
        }
        else if (index == 3)
        {
            GameObject e = Instantiate(openEntranceH, exitSpots[3].position, Quaternion.identity);
            exits[3] = true;
            e.transform.localScale = new Vector3(-1 * e.transform.localScale.x, 1 * e.transform.localScale.y, 1);
            doors.Add(e.transform.GetChild(0).GetComponent<Door>());
            if (doors[doors.Count - 1] == null) doors.Add(e.transform.GetChild(1).GetComponent<Door>());
        }
    }

    int EndPointToIndex(Vector2 p)
    {
        Log.LogMsg("Getting Dir/Index for point " + pos.x + ", " + pos.y + " to " + p.x + ", " + p.y);
        int i = -1;
        if (pos.x == p.x && pos.y > p.y) i = 2;
        else if (pos.x == p.x && pos.y < p.y) i = 0;
        else if (pos.x < p.x && pos.y == p.y) i = 1;
        else if (pos.x > p.x && pos.y == p.y) i = 3;

        Log.LogMsg("Index: " + i);
        return i;
    }

    public static Vector2 WorldToRoomPos(Vector2 p, Room r)
    {
        Vector2 oScale = r.scale;
        Vector2 b = new Vector2(r.pos.x * DungeonGenerator.ROOM_SPACING, r.pos.y * DungeonGenerator.ROOM_SPACING);

        //Log.LogMsg("Pos Conversion Log:");
        //Log.LogMsg("oScale: " + oScale.x + ", " + oScale.y);
        //Log.LogMsg("rPos: " + r.pos.x + ", " + r.pos.y);
        //Log.LogMsg("Room Spacing: " + DungeonGenerator.ROOM_SPACING);
        //Log.LogMsg("rOrigin: " + r.origin.x + ", " + r.origin.y);
        //Log.LogMsg("B: " + b.x + ", " + b.y);
        //Log.LogMsg("P: " + p.x + ", " + p.y);

        Vector2 v = new Vector2(p.x - b.x, p.y - b.y);
        //Log.LogMsg("V0: " + v.x + ", " + v.y);

        v = new Vector2(Mathf.Round(v.x - (r.origin.x * oScale.x)), Mathf.Round(v.y - (r.origin.y * oScale.y)));
        //Log.LogMsg("Subtractor: " + (r.origin.x * oScale.x) + ", " + (r.origin.y * oScale.y));
        //Log.LogMsg("Vfinal: " + v.x + ", " + v.y);

        return v;
    }

    public static Vector2 RoomToWorldPos(Vector2 p, Room r)
    {
        Vector2 oScale = r.scale;
        Vector2 b = new Vector2(r.pos.x * DungeonGenerator.ROOM_SPACING, r.pos.y * DungeonGenerator.ROOM_SPACING);
        Vector2 v = new Vector2(p.x + b.x, p.y + b.y);
        v = new Vector2(v.x + (r.origin.x * oScale.x), v.y + (r.origin.y * oScale.y));

        return v;
    }

    public void GenGrids()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (moveGrid[x][y])
                {
                    Vector2 start = RoomToWorldPos(new Vector2(x, y), this);
                    Vector2 p = new Vector2(player.transform.position.x, player.transform.position.y);
                    Vector2 dir = p - start;
                    //dir.Normalize();


                    RaycastHit2D hit = Physics2D.Raycast(start, dir, Mathf.Infinity);


                    if (hit.collider.name == "Player")
                    {
                        Log.LogMsg("PLAYER HIT! Raycast from " + start.x + ", " + start.y + " in direction " + dir.x + ", " + dir.y + ". Raycast hit " + hit.collider.name);
                        Log.LogMsg("Hit Position: " + hit.transform.position.x + ", " + hit.transform.position.y);
                        Log.LogMsg("Room Pos: " + x + ", " + y);
                        coverGrid[x][y] = false;
                        Debug.DrawRay(start, dir, Color.red, Options.OCCUPIED_ROOM_TICK_RATE + 0.02f);
                    }
                    else
                    {
                        Log.LogMsg("Raycast from " + start.x + ", " + start.y + " in direction " + dir.x + ", " + dir.y + ". Raycast hit " + hit.collider.name);
                        coverGrid[x][y] = true;
                        //Debug.DrawRay(start, dir, Color.yellow, Options.OCCUPIED_ROOM_TICK_RATE + 0.02f);
                    }
                }
                else coverGrid[x][y] = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.name == "Player") isPlayerInRoom = !isPlayerInRoom;
        if (isPlayerInRoom) Interval();
    }
}
