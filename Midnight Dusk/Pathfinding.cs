using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class Pathfinding
{
    public class Node 
    {
        public Vector2 pos, target;
        public Node parent;

        public int g, h, f; //g is dist from start, h is estimated dist from end, f is total of g + h

        public Node(Vector2 p, Node pa, Vector2 s, Vector2 t)
        {
            pos = p;
            parent = pa;
            target = t;

            if (parent != null) g = parent.g + 1;
            else g = 0;

            h = (int)(Math.Abs(t.x - p.x) + Math.Abs(t.y - p.y));

            f = g + h;
        }

        public void Update()
        {
            if (parent != null) g = parent.g + 1;
            else g = 0;

            h = (int)(Math.Abs(target.x - pos.x) + Math.Abs(target.y - pos.y));

            f = g + h;
        }
    }

    public static readonly Vector2[] DIRECTIONS = { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };

    public static void Pathfind(object param)
    {
        Log.LogMsg("Pathfind thread started successfully!");
        object[] paramArray = (object[])param;
        bool[][] grid = (bool[][])paramArray[0];
        Vector2 start = (Vector2)paramArray[1], target = (Vector2)paramArray[2];
        Room room = (Room)paramArray[3];
        Enemy enemy = (Enemy)paramArray[4];

        try
        {

            string s = "";
            for (int i = 0; i < grid[0].Length; i++)
            {
                if (s != "") s += "\n";
                for (int h = 0; h < grid.Length; h++)
                {
                    if (grid[h][i]) s += "T";
                    else s += "F";
                }
            }

            Log.LogMsg("Starting pathfinding...");
            Log.LogMsg("Grid: \n" + s);
            Log.LogMsg("Start: " + start.x + ", " + start.y);
            Log.LogMsg("Target: " + target.x + ", " + target.y);

            target = new Vector2(Mathf.Round(target.x), Mathf.Round(target.y));
            start = new Vector2(Mathf.Round(start.x), Mathf.Round(start.y));
            List<Node> closed = new List<Node>(), open = new List<Node>();
            List<Vector2> path = new List<Vector2>();
            Vector2 current = start, gridSize = new Vector2(grid.Length, grid[0].Length);
            closed.Add(new Node(current, null, start, target));
            Node currentNode = closed[closed.Count - 1];

            enemy.path = new List<Vector2>();
            enemy.pathfinding = true;

            Log.LogMsg("Initted PF vars");

            while (current != target)
            {
                Vector2[] dirs = GetPossibleDirections(current, grid);
                for (int i = 0; i < dirs.Length; i++)
                {
                    Vector2 n = Add(current, dirs[i]);
                    if ((int)n.x >= 0 && (int)n.x < grid.Length && (int)n.y >= 0 && (int)n.y < grid[0].Length)
                    {
                        if (grid[(int)n.x][(int)n.y])
                        {
                            if (ContainsNode(closed, n))
                            {
                                Log.LogMsg("Node is already in closed list, checking if it should be updated...");
                                int index = GetNodeIndex(closed, n);
                                if (closed[index].f > new Node(n, currentNode, start, target).f)
                                {
                                    Log.LogMsg("Updating node...");
                                    closed[index].parent = currentNode;
                                    closed[index].Update();
                                }
                            }
                            else
                                if (!ContainsNode(open, n)) open.Add(new Node(n, currentNode, start, target));
                            else
                            {
                                int index = GetNodeIndex(open, n);
                                open[index].parent = currentNode;
                                open[index].Update();
                            }
                            //Log.LogMsg("Added new node to open list");
                        }
                    }
                }

                for (int i = 0; i < open.Count; i++)
                {
                    int px = (int)open[i].pos.x;
                    int py = (int)open[i].pos.y;
                    if (!grid[px][py])
                    {
                        open.RemoveAt(i);
                        i--;
                        Debug.LogWarning("Removed impassable node from open list.");
                    }
                }

                int j = -1;
                for (int i = 0; i < open.Count; i++)
                {
                    if (j == -1) j = i;
                    else if (open[j].f >= open[i].f) j = i;
                }

                Node x = open[j];
                open.RemoveAt(j);
                closed.Add(x);
                current = x.pos;
                currentNode = x;
                path.Add(current);
                enemy.path.Add(Room.RoomToWorldPos(current, room));
                Log.LogMsg("Finished adding next step in path");

                Log.LogMsg("Path Length: " + path.Count);
                for (int i = 0; i < path.Count; i++)
                {
                    Log.LogMsg("Path[" + i + "]: " + path[i].x + ", " + path[i].y);
                }
            }

            for(int i = 0; i < path.Count; i++)
                path[i] = Room.RoomToWorldPos(path[i], room);

            //enemy.path = path;
            enemy.pathfinding = false;

            string p = "";
            for (int i = 0; i < enemy.path.Count; i++)
                p += grid[(int)enemy.path[i].x][(int)enemy.path[i].y] + "\n";
            Log.LogMsg("Path: " + p);

            Log.LogMsg("Finished pathfinding");
        }
        catch (Exception e)
        {
            //Log.LogException(e);
            enemy.pathfinding = false;
        }
    }

    public static Vector2[] GetPossibleDirections(Vector2 p, bool[][] grid)
    {
        List<Vector2> dirs = Enumerable.ToList(DIRECTIONS);

        //Log.LogMsg("Grid Length: " + grid.Length + ", " + grid[0].Length);

        for (int i = 0; i < DIRECTIONS.Length; i++)
        {
            Vector2 np = Add(p, DIRECTIONS[i]);
            if (!(np.x < 0) && !(np.x >= grid.Length))
                if(!(np.y < 0) && !(np.y >= grid[(int)np.x].Length))
                    if(grid[(int)np.x][(int)np.y])
                        if (i < DIRECTIONS.Length && i >= 0 && Vector2.Distance(p, np) < 1.4)
                        {
                            //Log.LogMsg("Adding direction at index " + i);
                            dirs.Add(DIRECTIONS[i]);
                        }
        }

        for(int i = 0; i < dirs.Count; i++)
        {
            if (i < dirs.Count)
            {
                Vector2 np = Add(p, dirs[i]);
                if (np.x >= 0 && np.x < grid.Length && np.y >= 0 && np.y < grid[0].Length)
                {
                    if (!grid[(int)np.x][(int)np.y]) dirs.RemoveAt(i);
                }
                //i--;
            }
        }

        return dirs.ToArray();
    }

    public static Vector2 Add(Vector2 x, Vector2 y)
    {
        return new Vector2(x.x + y.x, x.y + y.y);
    }

    public static bool ContainsNode(List<Node> l, Vector2 n)
    {
        for (int i = 0; i < l.Count; i++) if (l[i].pos == n) return true;

        return false;
    }

    public static int GetNodeIndex(List<Node> l, Vector2 n)
    {
        for(int i = 0; i < l.Count; i++) if (l[i].pos == n) return i;

        return -1;
    }
}
