using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Enemy>() != null)
        {
            other.gameObject.GetComponent<Enemy>().hp -= damage;
            if (other.gameObject.GetComponent<Enemy>().hp <= 0)
            {
                Destroy(gameObject);
                GameObject.Find("Game Manager").GetComponent<GameManager>().enemies--;
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().data += other.gameObject.GetComponent<Enemy>().data;
            }
        }
    }
}
