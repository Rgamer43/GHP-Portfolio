using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelect : MonoBehaviour
{

    public GameManager game;

    public int[] ids;
    public GameObject[] buttons;

    public GameObject infoUI;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("Game Manager").GetComponent<GameManager>();

        for(int x = 0; x < buttons.Length; x++)
        {
            GameObject buttonText = buttons[x].gameObject.transform.GetChild(0).gameObject;
            GameManager gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
            if (gm.uiNumber == 0 || gm.uiNumber == 1)
            {
                buttonText.gameObject.GetComponentInChildren<Text>().text = game.weapons[ids[x]].name + " - $" + game.weapons[ids[x]].cost;
            }
            else if(gm.uiNumber == 2)
            {
                buttonText.gameObject.GetComponentInChildren<Text>().text = game.armors[ids[x]].name + " - $" + game.armors[ids[x]].cost;
            }
        }

        //gameObject.transform.position = new Vector3(0, 0, 0);
        //gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClicked(int id)
    {
        print("Button was clicked");
        GameObject info = Instantiate(infoUI, new Vector3(0, 0, 0), Quaternion.identity);
        print("Instantiated info");

        info.gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
        info.transform.localPosition = new Vector3(0, 0, 0);
        if(game.uiNumber == 0 || game.uiNumber == 1) info.GetComponent<ItemInfo>().item = game.weapons[ids[id]];
        else if(game.uiNumber == 2) info.GetComponent<ItemInfo>().item = game.armors[ids[id]];

        Destroy(GameObject.Find("Button"));

        for(int x = 0; x < GameObject.FindGameObjectsWithTag("Button").Length; x++)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Button")[x]);
        }
        
        Destroy(gameObject);
    }
}
