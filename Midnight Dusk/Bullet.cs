using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bullet : MonoBehaviour
{

    public float velocity;
    public float damage;
    public float[] range;
    public Vector2 start;
    public bool player;
    //* Use this to hide the bullet but not the tracer
    public GameObject bulletSprite;

    public GameObject canvas;

    public GameObject damageMarkerFull;
    public GameObject damageMarkerPartial;
    public GameObject damageMarkerNone;

    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
        //damageMarkerFull = Resources.Load<GameObject>("Damage Marker - Full.prefab");
        //damageMarkerPartial = Resources.Load<GameObject>("Damage Marker - Partial Variant.prefab");
        //damageMarkerNone = Resources.Load<GameObject>("Damage Marker - None Variant.prefab");
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += transform.forward * velocity * Time.deltaTime;
    }

    public void Fire(float velocity, float damage, float[] range, bool player)
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right * velocity);
        this.velocity = velocity;
        this.damage = damage;
        this.range = range;
        this.player = player;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Log.LogMsg("Bullet hit " + collision.gameObject.name);
        OnCollide();

        Destroy(gameObject);

        if (collision.gameObject.GetComponent<FragGrenade>() != null) collision.gameObject.GetComponent<FragGrenade>().Explode();

        if(collision.gameObject.GetComponent<Enemy>() != null || collision.gameObject.GetComponent<Player>() != null)
        {
            float dist = Mathf.Abs(Vector2.Distance(start, transform.position));
            float dmg = damage;

            if (dist > range[1]) dmg = 0;
            else if(dist > range[0])
            {
                float d = (dist - range[0]);
                dmg *= 1 - (d / (range[1] - range[0]));
            }

            if(collision.gameObject.GetComponent<Enemy>() != null && player)
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(dmg, damage, collision.transform.position);
            }
            else if (collision.gameObject.GetComponent<Player>() != null && !player)
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(dmg, damage, collision.transform.position);
                OnHitPlayer();
            }

            print("Bullet Dist: " + dist + ", R0: " + range[0] + ", R1: " + range[1] + ", Dmg: " + dmg);
        }
    }

    public void OnCollide()
    {

    }

    public void OnHitPlayer()
    {

    }
}
