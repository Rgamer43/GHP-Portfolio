using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{

    public GameObject explosion;
    public float radius = 5f, explosionForce = 50f, explosionDamage = 25f;
    public Room room;
    public Vector2 pos;
    private bool hasExploded = false;

    // Start is called before the first frame update
    void Start()
    {
        pos = Room.WorldToRoomPos(transform.position, room);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Bullet>() != null) Explode();
    }

    public void Explode()
    {
        if (!hasExploded)
        {
            Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 5f);
            Destroy(gameObject);// Get nearby objects
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
            room.moveGrid[(int)pos.x][(int)pos.y] = true;
        }
    }
}
