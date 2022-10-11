using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : Interactable
{
    public override void OnInteract()
    {
        JobMenu.CreateJInfo(DungeonGenerator.instance.floor + 1);
        StartCoroutine(JobMenu.StartJob());
    }

    public override void OnStart()
    {
        if (DungeonGenerator.instance.floor >= DungeonGenerator.instance.floorCount) Destroy(gameObject);
    }
}
