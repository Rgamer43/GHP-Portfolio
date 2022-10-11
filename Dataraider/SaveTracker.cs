using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTracker : MonoBehaviour
{
    public static SaveTracker instance;

    public int id;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else 
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }
}
