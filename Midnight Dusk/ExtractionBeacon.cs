using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionBeacon : MenuOpener
{
    public void OnInteract()
    {
        SaveManager.Save();
        Destroy(player);
        base.OnInteract();
    }
}
