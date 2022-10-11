using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassOption : MonoBehaviour
{

    public ClassSelect select;
    public PlayerClass option;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        select.current.text = "Currently Selected: " + option;
        select.selected = option;
    }
}
