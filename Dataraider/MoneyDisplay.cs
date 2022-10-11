using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gameObject.GetComponent<Text>().text = "Money: " + gm.money;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
