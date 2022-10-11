using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Oddball : NetworkBehaviour
{

    public SpriteRenderer sprite;
    public int team;
    public Transform spawn;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            InvokeRepeating("AddPoints", 2, 2);
            spawn = GameObject.Find("Oddball Spawn").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetColorClientRPC(sprite.color);

        if(IsServer)
        {
            if (GameManager.score[0] >= 200 || GameManager.score[1] >= 200)
            {
                team = -1;
                sprite.color = Color.white;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                transform.position = spawn.position;
                GameManager.instance.oddControl.Value = team;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.GetComponent<Player>() != null)
            {
                team = collision.gameObject.GetComponent<Player>().handler.team.Value;

                if (team == 0) sprite.color = Color.blue;
                else sprite.color = Color.red;

                GameManager.instance.oddControl.Value = team;

                SetColorClientRPC(sprite.color);
            } else if(collision.gameObject.name == "Kill Zone")
            {
                team = -1;
                sprite.color = Color.white;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                transform.position = spawn.position;
                GameManager.instance.oddControl.Value = team;
            }
        }
    }

    public void AddPoints()
    {
        if(team >= 0 && team <= 1)
            GameManager.instance.AddPoints(team, 1);
    }

    [ClientRpc]
    public void SetColorClientRPC(Color color)
    {
        sprite.color = color;
    }
}
