using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    private void Start()
    {
        gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
        gameObject.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
    }

    public void Ready()
    {
        GameObject.Find("Game Manager").GetComponent<GameManager>().StartGame();
        Destroy(gameObject);
    }
}
