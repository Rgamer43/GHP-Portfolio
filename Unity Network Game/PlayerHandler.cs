using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHandler : NetworkBehaviour
{

    public Player player;
    public GameObject character, canvasObject;
    public Camera camera;
    public ClassSelect classSelect;
    public GameObject HUD;

    public NetworkVariable<int> team = new NetworkVariable<int>();

    // Start is called before the first frame update
    void Start()
    {
        player = character.GetComponent<Player>();
        player.handler = this;

        player.Disable();
        camera.enabled = false;
        canvasObject.SetActive(false);
        HUD.SetActive(false);

        if(IsOwner)
        {
            camera.gameObject.GetComponent<AudioListener>().enabled = true;
            camera.enabled = true;
            canvasObject.SetActive(true);
            classSelect.handler = this;
            classSelect.gameObject.SetActive(true);
        }

        if (IsServer)
        {
            team.Value = -1;
            GameManager.instance.UpdateLocalScoreClientRPC(GameManager.score);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(team.Value == -1 && IsServer)
        {
            if (GameManager.GetTeamSize(0) > GameManager.GetTeamSize(1))
                team.Value = 1;
            else team.Value = 0;
            GameManager.instance.AddToTeam(team.Value, OwnerClientId);
            print("Player " + OwnerClientId + " joining team " + team.Value + ". Team Sizes are now: " + GameManager.GetTeamSize(0) + " - " + GameManager.GetTeamSize(1));


            if (team.Value == 0) player.spawn = GameObject.Find("Blue Spawn").transform;
            else player.spawn = GameObject.Find("Red Spawn").transform;

            character.transform.position = player.spawn.position;
        }
    }

    [ClientRpc]
    public void DeathClientRPC()
    {
        if (IsOwner)
        {
            print("Triggered death RPC on owner.");
            player.Disable();
            HUD.SetActive(false);
            classSelect.gameObject.SetActive(true);

            //if (team.Value == 0) GameManager.instance.AddPointsServerRPC(1, 1);
            //else GameManager.instance.AddPointsServerRPC(0, 1);
        }
        else print("Triggered death RPC on non-owner");
    }
}
