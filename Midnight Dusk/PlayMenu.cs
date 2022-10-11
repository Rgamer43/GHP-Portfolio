using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;

public class PlayMenu : MonoBehaviour
{

    public string[] saves;
    public GameObject saveObject;
    public List<GameObject> saveButtons = new List<GameObject>();

    public GameObject content;
    public Transform contentStart;

    public GameObject newGame;

    // Start is called before the first frame update
    void Start()
    {
        saves = SaveManager.GetSaves();

        for(int i = 0; i < saves.Length; i++)
        {
            GameObject s = Instantiate(saveObject, new Vector3(0, 0, 0), Quaternion.identity);
            s.transform.SetParent(GameObject.Find("Save Options").transform);
            //s.transform.position = new Vector3(0, i * 80, 0);
            s.transform.localPosition = new Vector3(0, i * -93 + 330, 0);
            s.transform.name = saves[i];
            s.transform.GetChild(1).GetComponent<Text>().text = saves[i];
            saveButtons.Add(s);
        }
    }

    void Update()
    {
        if (content.transform.position.y < contentStart.position.y) content.transform.position = contentStart.position;
    }

    public void Back()
    {
        Destroy(gameObject);
    }

    public void NewGame()
    {
        Instantiate(newGame, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject.Find("New Game(Clone)").GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        GameObject.Find("New Game(Clone)").GetComponent<Canvas>().sortingOrder = 1;
    }
}
