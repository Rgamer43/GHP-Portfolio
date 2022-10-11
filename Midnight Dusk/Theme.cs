using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Theme
{

    public string name;

    public List<GameObject> rooms = new List<GameObject>();
    public List<GameObject> starts = new List<GameObject>();
    public List<GameObject> ends = new List<GameObject>();
    public List<GameObject> hallwaysV = new List<GameObject>();
    public List<GameObject> hallwaysH = new List<GameObject>();

    public Theme(string name)
    {
        this.name = name;

        string[] roomFiles = File.ReadAllLines(Application.dataPath + "/Resources/Themes/" + name + "/Rooms/assetlist.txt");
        for (int i = 0; i < roomFiles.Length; i++)
            rooms.Add(Resources.Load<GameObject>("Themes/" + name + "/Rooms/" + roomFiles[i]));

        roomFiles = File.ReadAllLines(Application.dataPath + "/Resources/Themes/" + name + "/Starts/assetlist.txt");
        for (int i = 0; i < roomFiles.Length; i++)
            starts.Add(Resources.Load<GameObject>("Themes/" + name + "/Starts/" + roomFiles[i]));

        roomFiles = File.ReadAllLines(Application.dataPath + "/Resources/Themes/" + name + "/Ends/assetlist.txt");
        for (int i = 0; i < roomFiles.Length; i++)
            ends.Add(Resources.Load<GameObject>("Themes/" + name + "/Ends/" + roomFiles[i]));

        roomFiles = File.ReadAllLines(Application.dataPath + "/Resources/Themes/" + name + "/HallwaysV/assetlist.txt");
        for (int i = 0; i < roomFiles.Length; i++)
            hallwaysV.Add(Resources.Load<GameObject>("Themes/" + name + "/HallwaysV/" + roomFiles[i]));

        roomFiles = File.ReadAllLines(Application.dataPath + "/Resources/Themes/" + name + "/HallwaysH/assetlist.txt");
        for (int i = 0; i < roomFiles.Length; i++)
            hallwaysH.Add(Resources.Load<GameObject>("Themes/" + name + "/HallwaysH/" + roomFiles[i]));
    }
}
