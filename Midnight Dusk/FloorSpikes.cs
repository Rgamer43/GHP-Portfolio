using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpikes : MonoBehaviour
{
    public float damage, extendTime, retractTime;
    public SpriteRenderer sprite;
    public Sprite extended, retracted;
    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = retracted;
        active = false;
        Invoke("Extend", retractTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Extend()
    {
        active = true;
        sprite.sprite = extended;
        Invoke("Retract", extendTime);
    }

    public void Retract()
    {
        active = false;
        sprite.sprite = retracted;
        Invoke("Extend", retractTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Log.LogMsg("Spikes triggered");
        if(active)
        {
            if (collision.GetComponent<Enemy>() != null) collision.gameObject.GetComponent<Enemy>().TakeDamage(damage, collision.transform.position);
            else if (collision.GetComponent<Player>() != null) collision.gameObject.GetComponent<Player>().TakeDamage(damage, collision.transform.position);
        }
    }
}
