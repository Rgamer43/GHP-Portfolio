using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hill : NetworkBehaviour
{

    public int[] players = new int[2];
    public SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer) InvokeRepeating("AddPoints", 2, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (players[0] > players[1])
            {
                sprite.color = Color.blue;
                GameManager.instance.hillControl.Value = 0;
            }
            else if (players[0] < players[1]) 
            { 
                sprite.color = Color.red;
                GameManager.instance.hillControl.Value = 1;
            }
            else { 
                sprite.color = Color.white;
                if (players[0] > 0) GameManager.instance.hillControl.Value = -2;
                else GameManager.instance.hillControl.Value = -1;
            }

            SetColorClientRPC(sprite.color);
        }
    }

    [ClientRpc]
    public void SetColorClientRPC(Color color)
    {
        sprite.color = color;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsServer)
        {
            if (collision.GetComponent<Player>() != null) players[collision.GetComponent<Player>().handler.team.Value] += 1;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (IsServer)
        {
            if (collision.GetComponent<Player>() != null) players[collision.GetComponent<Player>().handler.team.Value] -= 1;
        }
    }

    public void AddPoints()
    {
        if (players[0] > players[1])
        {
            GameManager.instance.AddPoints(0, 1);
        }
        else if (players[0] < players[1]) { GameManager.instance.AddPoints(1, 1); }
    }
}
