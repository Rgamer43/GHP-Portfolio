using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public Animator animator;
    public BoxCollider2D collider;
    public Transform player;

    public bool open = true;
    public bool locked = false;

    public int playerInRange = 0;

    public static readonly float OPEN_DIST = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = true;
        EnableCollider();
        Invoke("Close", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (player == null)
                player = GameObject.Find("Player").transform;
        

            if (open) DisableCollider();

            if (locked)
            {
                playerInRange = 0;
                Close();
            }
            else
            {
                //Log.LogMsg("Dist to Player: " + Vector2.Distance(player.position, transform.position));
                if (Vector2.Distance(player.position, transform.position) > OPEN_DIST)
                {
                    if (playerInRange == 1) Close();
                    playerInRange = 0;
                }
                else
                {
                    if (playerInRange == 0)
                    {
                        playerInRange = 1;
                        Open();
                    }
                }
            }
        }
        catch
        {

        }
    }

    public void Open()
    {
        if (!locked)
        {
            Log.LogImportant("Opening door...");
            open = true;
            animator.SetTrigger("Open");
            collider.enabled = false;
            //Invoke("DisableCollider", 1.25f);
        }
    }

    public void Close()
    {
        if (open || locked || true)
        {
            Log.LogImportant("Closing door...");
            open = false;
            animator.SetTrigger("Close");
            Invoke("EnableCollider", 0.75f);
        }
    }

    public void EnableCollider()
    {
        collider.enabled = true;
        //if (open) Close();
    }

    public void DisableCollider()
    {
        collider.enabled = false;
        //if (!open) Open();
    }
}
