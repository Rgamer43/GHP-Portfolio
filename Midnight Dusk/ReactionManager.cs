using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReactionManager
{
    
    public static void Explode(Collider2D[] objects, GameObject caller)
    {
        Log.LogMsg("Exploding via ReactionManager...");
        foreach(Collider2D i in objects)
        {
            if (i.gameObject != caller)
            {
                if (i.GetComponent<FragGrenade>() != null) i.GetComponent<FragGrenade>().Explode();
                else if (i.GetComponent<ExplosiveBarrel>() != null) i.GetComponent<ExplosiveBarrel>().Explode();
            }
        }
    }

}
