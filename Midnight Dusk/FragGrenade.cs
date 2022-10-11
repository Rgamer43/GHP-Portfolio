using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragGrenade : MonoBehaviour
{

    public float throwVelocity;
    public float timeToExplode;
    public float explosionDamage;
    public float explosionForce = 50f;
    public float radius = 5f;
    public GameObject explosionEffect;

    private float countdown;
    private bool hasExploded = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * throwVelocity);
        countdown = timeToExplode;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0 && !hasExploded){
            Explode();
            hasExploded = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Enemy>() != null)
        {
            Explode();
        }
    }

    public void Explode() {
        if (!hasExploded)
        {
            //Show effect
            Destroy(Instantiate(explosionEffect, transform.position, Quaternion.identity), 5f);
            // Get nearby objects
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            Debug.Log(colliders.Length);
            foreach (Collider2D nearbyObject in colliders)
            {
                Rigidbody2D rb = nearbyObject.GetComponent<Rigidbody2D>();
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (nearbyObject.transform.position - transform.position), radius);
                if (rb != null)
                {
                    Debug.Log(nearbyObject.gameObject.name);
                    rb.AddExplosionForce(explosionForce, transform.position, radius);
                    if (nearbyObject.gameObject.GetComponent<Enemy>() != null)
                    {
                        nearbyObject.gameObject.GetComponent<Enemy>().TakeDamage(explosionDamage, explosionDamage, hit.transform.position);
                    }
                    else if (nearbyObject.gameObject.GetComponent<Player>() != null)
                    {
                        nearbyObject.gameObject.GetComponent<Player>().TakeDamage(explosionDamage, explosionDamage, hit.transform.position);
                    }
                }
            }

            hasExploded = true;
            ReactionManager.Explode(colliders, gameObject);
            // Add force
            // Damage
            Destroy(gameObject);
        }
    }
}
