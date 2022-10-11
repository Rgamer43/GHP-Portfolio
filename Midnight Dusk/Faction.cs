using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Faction
{

    public string name;
    public int power;

    public List<GameObject> enemies = new List<GameObject>();
    public List<float> enemyPowers = new List<float>();
    public List<int> minSpawning = new List<int>();
    public List<int> maxSpawning = new List<int>();

    public Faction(string n)
    {
        name = n;

        string[] enemyFiles = File.ReadAllLines(Application.dataPath + "/Resources/Factions/" + name + "/Enemies/assetlist.txt");
        for (int i = 0; i < enemyFiles.Length; i++)
        {
            enemies.Add(Resources.Load<GameObject>("Factions/" + name + "/Enemies/" + enemyFiles[i].Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries)[0]));
            enemyPowers.Add(float.Parse(enemyFiles[i].Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries)[1]));
            minSpawning.Add(int.Parse(enemyFiles[i].Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries)[2]));
            maxSpawning.Add(int.Parse(enemyFiles[i].Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries)[3]));
        }
    }

}
