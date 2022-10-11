using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{

    class Point
    {
        public Vector2 pos;
        public string marking = "";

        public Point(Vector2 p)
        {
            pos = p;
        }

        public Point(Vector2 p, string m)
        {
            pos = p;
            marking = m;
        }

        public Point(int x, int y)
        {
            pos = new Vector2(x, y);
        }

        public Point(int x, int y, string m)
        {
            pos = new Vector2(x, y);
            marking = m;
        }
    }

    class Hallway
    {
        public Vector2[] endpoints;
        public Hallway(Vector2 s, Vector2 e)
        {
            endpoints = new Vector2[] { s, e };
        }

        public Hallway(int x1, int y1, int x2, int y2)
        {
            endpoints = new Vector2[]
            {
                new Vector2(x1, y2), new Vector2(x2, y2)
            };
        }
    }

    [SerializeField]
    public Faction faction;
    public Theme theme;

    public static DungeonGenerator instance;

    private readonly int DUNGEON_SIZE = 7;
    private Point[][] points;
    private List<Hallway> hallways = new List<Hallway>();

    //Config
    private readonly int[] TOTAL_ROOM_RANGE = { 20, 45 };
    private readonly float MIN_START_TO_END_DIST = 3f;
    private readonly float ROOM_CHANCE = 0.4f;
    private readonly float HALLWAY_CHANCE = 0.2f;
    public static readonly int ROOM_SPACING = Options.DUNGEON_ROOM_SPACING;

    public GameObject player;

    private readonly Vector2[] DIRECTIONS =
    {
        new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0)
        //North, East, South, West
    };

    private int roomsGenerated = 0;
    private List<Vector2> roomsToGenerate = new List<Vector2>();

    public int floor, floorCount;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) instance = this;

        //Load faction and theme
        GameObject jobInfo = GameObject.FindGameObjectWithTag("Job Info");
        string[] jInfo = jobInfo.name.Split(' ');
        faction = FactionList.factions[int.Parse(jInfo[0])];
        theme = ThemeList.themes[int.Parse(jInfo[1])];
        floorCount = int.Parse(jInfo[2]);
        floor = int.Parse(jInfo[3]);
        Destroy(jobInfo);

        GenDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenDungeon()
    {
        //Init points
        points = new Point[DUNGEON_SIZE][];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Point[DUNGEON_SIZE];
            for (int j = 0; j < points[i].Length; j++)
                points[i][j] = new Point(i, j);
        }

        int totalRooms = Random.Range(TOTAL_ROOM_RANGE[0], TOTAL_ROOM_RANGE[1]);

        Vector2 start = new Vector2(Random.Range(0, DUNGEON_SIZE), Random.Range(0, DUNGEON_SIZE));
        points[(int)start.x][(int)start.y].marking = "start";

        Log.LogImportant("Generating rooms...");
        GenRooms(start);
        Log.LogImportant("Finished generating rooms");

        int attempts = 0;

        while(roomsGenerated < totalRooms)
        {
            if (attempts >= 1000 && roomsGenerated > 10) break;

            if (roomsToGenerate.Count == 0)
            {
                Log.LogWarning("No rooms to generate");
                attempts = 1000;
                break;
            }
            Shuffle(roomsToGenerate);
            Log.LogMsg("Attempts: " + attempts);
            Log.LogMsg("Rooms to Generate: " + roomsToGenerate.Count);
            Log.LogMsg("Generated Rooms: " + roomsGenerated);
            GenRooms(roomsToGenerate[0]);
            attempts++;
        }

        Log.LogMsg("Finished initial room generation");
        Log.LogMsg("Attempts made: " + attempts);
        Log.LogMsg("Rooms Generated: " + roomsGenerated);
        if (attempts >= 1000) Log.LogError("Attempts exceeded maximum");

        try
        {

            Vector2 end = new Vector2(Random.Range(0, DUNGEON_SIZE), Random.Range(0, DUNGEON_SIZE));

            attempts = 0;
            while ((Vector2.Distance(start, end) < MIN_START_TO_END_DIST && attempts < 1000) || points[(int)end.x][(int)end.y].marking != "room")
            {
                Log.LogMsg("Attempting to pick end point");
                end = new Vector2(Random.Range(0, DUNGEON_SIZE), Random.Range(0, DUNGEON_SIZE));
                attempts++;
            }
            points[(int)end.x][(int)end.y].marking = "end";

            Log.LogMsg("Attempts Made to Pick End: " + attempts);

            Log.LogMsg("Picked end point");

            attempts = 0;
            //Add additional hallways
            for (int i = 0; i < points.Length && attempts < Options.ATTEMPTS_LIMIT_MAJOR; i++)
                for (int j = 0; j < points[i].Length && attempts < Options.ATTEMPTS_LIMIT_MAJOR; j++)
                {
                    Log.LogMsg("Generating additional hallways for " + i + ", " + j);
                    Vector2 p = new Vector2(i, j);
                    Vector2[] dirs = GetPossibleDirectionsIncludingRooms(new Vector2(i, j));
                    if (points[(int)p.x][(int)p.y].marking != "")
                    {
                        Log.LogMsg("Checking for possibility of hallways at " + p.x + ", " + p.y);
                        Log.LogMsg("# of Potential Directions: " + dirs.Length);
                        for (int h = 0; h < dirs.Length; h++)
                        {
                            if (!hallways.Contains(new Hallway(p, Add(p, dirs[h]))) //Check if hallway already exists 
                                && !hallways.Contains(new Hallway(Add(p, dirs[h]), p))) //Check if hallways exists in other direction
                            {
                                Log.LogMsg("Checking if there's something at " + Add(p, dirs[h]).x + ", " + Add(p, dirs[h]).y);
                                if (points[(int)Add(p, dirs[h]).x][(int)Add(p, dirs[h]).y].marking != "") //Check if there's something at the other point
                                {
                                    Log.LogMsg("Potential hallway between " + p.x + ", " + p.y + " and " + Add(p, dirs[h]).x + ", " + Add(p, dirs[h]).y);
                                    if (Random.Range(0.0f, 1.0f) < HALLWAY_CHANCE)
                                    {
                                        Log.LogMsg("Added hallway between " + p.x + ", " + p.y + " and " + Add(p, dirs[h]).x + ", " + Add(p, dirs[h]).y);
                                        hallways.Add(new Hallway(p, Add(p, dirs[h])));
                                    }
                                    attempts++;
                                }
                                else Log.LogMsg("Nothing at " + Add(p, dirs[h]).x + ", " + Add(p, dirs[h]).y);
                            }
                            else Log.LogMsg("Hallway already exists");
                        }
                    } Log.LogMsg(p.x + ", " + p.y + " is empty");
                }
            Log.LogImportant("Finished adding additional hallways");

            Log.LogImportant("Spawning rooms...");
            for (int i = 0; i < points.Length; i++)
                for (int j = 0; j < points[i].Length; j++)
                {
                    if (points[i][j].marking == "room")
                    {
                        Log.LogMsg("Spawning room for " + i + ", " + j);
                        GameObject r = Instantiate(theme.rooms[Random.Range(0, theme.rooms.Count)], new Vector2(i * ROOM_SPACING, j * ROOM_SPACING), Quaternion.identity);
                        r.name = "Room " + i + " " + j;
                        r.GetComponent<Room>().SetPos(new Vector2(i, j));
                    }
                }
            Log.LogImportant("Finished spawning rooms");

            Log.LogMsg("Spawning entrance and exit...");
            Log.LogMsg("Count of Starts: " + theme.starts.Count);
            Log.LogMsg("Count of Ends: " + theme.ends.Count);

            GameObject s = Instantiate(theme.starts[Random.Range(0, theme.starts.Count)], new Vector2(start.x * ROOM_SPACING, start.y * ROOM_SPACING), Quaternion.identity);
            s.name = "Room " + start.x + " " + start.y;
            GameObject.Find("Room " + start.x + " " + start.y).GetComponent<Room>().SetPos(new Vector2(start.x, start.y));

            GameObject e = Instantiate(theme.ends[Random.Range(0, theme.ends.Count)], new Vector2(end.x * ROOM_SPACING, end.y * ROOM_SPACING), Quaternion.identity);
            e.name = "Room " + end.x + " " + end.y;
            GameObject.Find("Room " + end.x + " " + end.y).GetComponent<Room>().SetPos(new Vector2(end.x, end.y));

            Log.LogImportant("Finished spawning entrance and exit");

            Log.LogImportant("Spawning hallways...");
            for (int i = 0; i < hallways.Count; i++)
            {
                Hallway h = hallways[i];
                Log.LogMsg("Spawning hallway from " + h.endpoints[0].x + ", " + h.endpoints[1].y + " to " + h.endpoints[1].x + ", " + h.endpoints[1].y);
                Vector2 p = new Vector2(((h.endpoints[0].x + h.endpoints[1].x) / 2)*ROOM_SPACING, ((h.endpoints[0].y + h.endpoints[1].y) / 2)*ROOM_SPACING);
                if (h.endpoints[0].y != h.endpoints[1].y) Instantiate(theme.hallwaysV[Random.Range(0, theme.hallwaysV.Count)], p, Quaternion.identity);
                else Instantiate(theme.hallwaysH[Random.Range(0, theme.hallwaysH.Count)], p, Quaternion.identity);

                GameObject.Find("Room " + h.endpoints[0].x + " " + h.endpoints[0].y).GetComponent<Room>().AddEntrance(h.endpoints[1]);
                GameObject.Find("Room " + h.endpoints[1].x + " " + h.endpoints[1].y).GetComponent<Room>().AddEntrance(h.endpoints[0]);
            }
            Log.LogImportant("Finished spawning hallways");

            Log.LogImportant("Initializing rooms...");
            for (int i = 0; i < points.Length; i++)
                for (int j = 0; j < points[i].Length; j++)
                {
                    if (points[i][j].marking != "")
                    {
                        Log.LogMsg("Initializing room " + i + ", " + j);
                        GameObject.Find("Room " + i + " " + j).GetComponent<Room>().Init();
                    }
                }
            Log.LogImportant("Finished initting rooms");

            Log.LogImportant("Spawning player");
            Room o = GameObject.Find("Room " + start.x + " " + start.y).GetComponent<Room>();
            Vector2 spawn = Room.RoomToWorldPos(new Vector2(o.origin.x, o.origin.y), o);

            Log.LogImportant("Player Spawn: " + spawn.x + ", " + spawn.y);

            Instantiate(player, spawn, Quaternion.identity);
        }
        catch (System.ArgumentOutOfRangeException e)
        {
            //Log.LogWarning(e.ToString());
        }
    }

    void GenRooms(Vector2 p)
    {
        Log.LogMsg("Generating rooms for " + p.x + ", " + p.y);
        Vector2[] dirs = GetPossibleDirections(p);
        List<Vector2> gennedRooms = new List<Vector2>();

        Log.LogMsg("# of Possible Directions: " + dirs.Length);

        while (true)
        {
            int attempts = 0;
            for (int i = 0; i < dirs.Length && attempts < Options.ATTEMPTS_LIMIT_MAJOR; i++)
            {
                if (Random.Range(0.0f, 1.0f) < ROOM_CHANCE)
                {
                    Vector2 r = Add(p, dirs[i]);
                    points[(int)r.x][(int)r.y].marking = "room";
                    gennedRooms.Add(r);
                    hallways.Add(new Hallway(p, r));
                    roomsToGenerate.Add(r);
                    Log.LogMsg("Adding room at " + r.x + ", " + r.y);
                }
                attempts++;
            }

            if (points[(int)p.x][(int)p.y].marking != "start" || gennedRooms.Count > 0) break;
        }

        roomsGenerated++;

        if (gennedRooms.Count != 0) roomsToGenerate.Remove(p);
    }

    Vector2[] GetPossibleDirections(Vector2 p)
    {
        List<Vector2> dirs = Enumerable.ToList(DIRECTIONS);

        for(int i = 0; i < DIRECTIONS.Length; i++)
        {
            Vector2 np = Add(p, DIRECTIONS[i]);
            if ((np.x < 0 || np.x >= DUNGEON_SIZE || np.y < 0 || np.y >= DUNGEON_SIZE) || points[(int)np.x][(int)np.y].marking != "")
                if (i < DIRECTIONS.Length && i >= 0)
                    dirs.Remove(DIRECTIONS[i]);
        }

        Shuffle(dirs);

        return dirs.ToArray();
    }

    Vector2[] GetPossibleDirectionsIncludingRooms(Vector2 p)
    {
        List<Vector2> dirs = Enumerable.ToList(DIRECTIONS);

        for (int i = 0; i < DIRECTIONS.Length; i++)
        {
            Vector2 np = Add(p, DIRECTIONS[i]);
            if ((np.x < 0 || np.x >= DUNGEON_SIZE || np.y < 0 || np.y >= DUNGEON_SIZE))
                if (i < DIRECTIONS.Length && i >= 0)
                    dirs.Remove(DIRECTIONS[i]);
        }

        Shuffle(dirs);

        return dirs.ToArray();
    }

    public Vector2 Add(Vector2 x, Vector2 y)
    {
        return new Vector2(x.x + y.x, x.y + y.y);
    }

    void Shuffle(List<Vector2> list)
    {
        int n = list.Count;
        Log.LogMsg("Shuffling...");
        int attempts = 0;
        while (n > 1 && attempts < Options.ATTEMPTS_LIMIT_MINOR)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            Vector2 value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        Log.LogMsg("Finished shuffling");
    }
}
