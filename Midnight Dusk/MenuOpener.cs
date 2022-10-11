using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOpener : Interactable
{

    public GameObject menu;

    public override void OnInteract()
    {
        Instantiate(menu, new Vector3(0, 0, 0), Quaternion.identity);
        Player.instance.controlsEnabled = false;
    }

    public override void OnStart()
    {
        
    }
}
