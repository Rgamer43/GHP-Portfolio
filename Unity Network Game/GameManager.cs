using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static List<ulong>[] teams = new List<ulong>[2];
    public static int[] score = new int[2];
    public NetworkVariable<int> blueCount = new NetworkVariable<int>(), redCount = new NetworkVariable<int>(), 
        blueScore = new NetworkVariable<int>(), redScore = new NetworkVariable<int>(), hillControl = new NetworkVariable<int>(), oddControl = new NetworkVariable<int>();

    public static GameManager instance;

    public Transform oddballSpawn, hillSpawn;
    public GameObject oddball, hill;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer && instance == null)
        {
            instance = this;

            try
            {
                GetComponent<NetworkObject>().Spawn();
            } catch (Exception e)
            {
                Debug.LogException(e);
            }

            blueCount = new NetworkVariable<int>();
            redCount = new NetworkVariable<int>();
            blueScore = new NetworkVariable<int>();
            redScore = new NetworkVariable<int>();

            score[0] = 0;
            score[1] = 0;
            blueCount.Value = 0;
            redCount.Value = 0;
            blueScore.Value = 0;
            redScore.Value = 0;

            teams[0] = new List<ulong>();
            teams[1] = new List<ulong>();

            //oddballSpawn = GameObject.FindGameObjectWithTag("Oddball Spawn").transform;
            print("Spawning Oddball... Prefab: " + oddball.ToString() + ", Spawn: " + oddballSpawn.ToString());
            GameObject o = Instantiate(oddball, oddballSpawn.position, Quaternion.identity);
            print("Instantiated oddball. GameObject: " + o.ToString() + ". Spawning NetworkObject...");
            o.GetComponent<NetworkObject>().Spawn();
            print("Spawned oddball");

            GameObject h = Instantiate(hill, hillSpawn.position, Quaternion.identity);
            h.GetComponent<NetworkObject>().Spawn();
        }
        else if(instance != null) Destroy(gameObject);

        if (instance == null) instance = this;

        //gameObject.SetActive(false);

        teams[0] = new List<ulong>();
        teams[1] = new List<ulong>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer)
        {
            if (teams[0] == null)
            {
                Debug.LogWarning("Requested team is null! Team: " + 0);
                teams[0] = new List<ulong>();
                print("Initialized team...");
                if (teams[0] == null) Debug.LogWarning("Team is still null!");
                else print("Team initted successfully");
            }
            if (teams[1] == null)
            {
                Debug.LogWarning("Requested team is null! Team: " + 1);
                teams[1] = new List<ulong>();
                print("Initialized team...");
                if (teams[1] == null) Debug.LogWarning("Team is still null!");
                else print("Team initted successfully");
            }

            if (score[0] == null)
            {
                Debug.LogWarning("Requested score is null! Team: " + 0);
                score[0] = 0;
                print("Initialized score...");
                if (score[0] == null) Debug.LogWarning("Score is still null!");
                else print("Score initted successfully");
            }
            if (score[1] == null)
            {
                Debug.LogWarning("Requested score is null! Team: " + 1);
                score[1] = 0;
                print("Initialized score...");
                if (score[1] == null) Debug.LogWarning("Score is still null!");
                else print("Score initted successfully");
            }

            blueCount.Value = teams[0].Count;
            redCount.Value = teams[1].Count;
            blueScore.Value = score[0];
            redScore.Value = score[1];
        }
    }

    public void AddToTeam(int team, ulong id)
    {
        print("Teams: " + teams.ToString() + ", length: " + teams.Length);
        if (teams[team] == null)
        {
            Debug.LogWarning("Requested team is null! Team: " + team);
            teams[team] = new List<ulong>();
            print("Initialized team...");
            if (teams[team] == null) Debug.LogWarning("Team is still null!");
            else print("Team initted successfully");
        }
        teams[team].Add(id);
        blueCount.Value = teams[0].Count;
        redCount.Value = teams[1].Count;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPointsServerRPC(int team, int points)
    {
        AddPoints(team, points);
    }

    public void AddPoints(int team, int points)
    {
        score[team] += points;
        print("Added " + points + " points to team " + team);
        blueScore.Value = score[0];
        redScore.Value = score[1];
        print("Blue: " + blueScore.Value + ", Red: " + redScore.Value);

        UpdateLocalScoreClientRPC(score);
    }

    public static int GetTeamSize(int team)
    {
        if (instance == null)
        {
            Debug.LogWarning("GameManager method called when instance is null! Method Called: GetTeamSize(" + team + ")");
            return 0;
        }

        if (team == 0) return instance.blueCount.Value;
        else return instance.redCount.Value;
    }

    [ClientRpc]
    public void UpdateLocalScoreClientRPC(int[] s)
    {
        score = s;
        print("Score: " + score[0] + " - " + score[1]);
    }
}
