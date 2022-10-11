using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{

    public GameObject player;
    public SpriteRenderer border;

    public float fadeSpeed = 2;
    public float range = 1.5f;

    public string name;
    public string verb;

    private bool isInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) player = GameObject.Find("Player");
        else if(Vector2.Distance(player.transform.position, transform.position) < range)
        {
            if (!isInRange)
            {
                GameObject.Find("Interaction").GetComponent<Text>().text = "Press " + Keybindings.interact.ToString() + " to " + verb + " " + name;
                isInRange = true;
            }

            if (Input.GetKeyDown(Keybindings.interact)) OnInteract();
        }
        else
        {
            if (isInRange && GameObject.Find("Interaction").GetComponent<Text>().text == "Press " + Keybindings.interact.ToString() + " to " + verb + " " + name)
            {
                GameObject.Find("Interaction").GetComponent<Text>().text = "";
                isInRange = false;
            }
        }

        border.color = Color.Lerp(Color.black, Color.yellow, Mathf.PingPong(Time.time, fadeSpeed)/fadeSpeed);
    }

    public abstract void OnStart();
    public abstract void OnInteract();
}
