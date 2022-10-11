using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuExitter : MonoBehaviour
{

    public GameObject destroyedObject;
    public bool unlockControls;

    public void Exit()
    {
        Destroy(destroyedObject);
        if (Player.instance != null && unlockControls) Player.instance.controlsEnabled = true;
    }
}
