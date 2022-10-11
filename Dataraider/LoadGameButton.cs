using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameButton : MonoBehaviour
{

    public int id;

    // Start is called before the first frame update
    void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/save" + id + ".save"))
            gameObject.GetComponent<Button>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
