using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRescaler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = gameObject.GetComponent<RectTransform>().transform.position;
        float width = (Screen.width / 2) / pos.x;
        float height = (Screen.height / 2) / pos.y;
        gameObject.GetComponent<RectTransform>().transform.position = new Vector3(Screen.width*width, Screen.height*height,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
