using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub : MonoBehaviour
{

    public GameObject playerPrefab;
    public Transform playerSpawn;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("Player") == null)
        {
            player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
            player.name = "Player";
        }
        else player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
