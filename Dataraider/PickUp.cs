using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{

    public int[] health;
    public int[] ammo;

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
        if(other.gameObject.tag == "Player")
        {
            Player p = other.gameObject.GetComponent<Player>();
            p.Heal(Random.Range(health[0], health[1]));
            p.weapon[0].unloadedAmmo += p.weapon[0].ammoPerMag * Random.Range(ammo[0], ammo[1]);
            p.weapon[1].unloadedAmmo += p.weapon[1].ammoPerMag * Random.Range(ammo[0], ammo[1]);
            Destroy(gameObject);
        }
    }
}
