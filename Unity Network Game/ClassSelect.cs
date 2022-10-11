using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelect : MonoBehaviour
{

    public Dictionary<PlayerClass, string> classes = new Dictionary<PlayerClass, string>()
    {
        { PlayerClass.Soldier, "A standard combatant" },
        { PlayerClass.Sniper, "A lethal, but fragile, attacker" },
        { PlayerClass.Raider, "A deadly attacker" },
        { PlayerClass.Defender, "A stalwart guardian" }
    };

    public GameObject content, option, HUD;
    public Text current;
    public PlayerClass selected;
    public PlayerHandler handler;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < classes.Count; i++)
        {
            GameObject o = Instantiate(option, content.transform);
            o.GetComponent<ClassOption>().select = this;
            o.GetComponent<ClassOption>().option = classes.ElementAt(i).Key;
            o.transform.GetChild(0).GetComponent<Text>().text = classes.ElementAt(i).Key.ToString() + ": " + classes.ElementAt(i).Value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy) HUD.SetActive(false);
    }

    public void Play()
    {
        handler.player.Enable();
        handler.player.SetClassServerRPC(selected);
        gameObject.SetActive(false);
    }
}
