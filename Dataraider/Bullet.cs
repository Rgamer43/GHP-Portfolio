using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float damage;
    public float speed;
    public float lifetime;
    protected Rigidbody rb;
    public bool damagePlayer;

    public Transform crosshairPos;
    public GameObject hitMarker;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        crosshairPos = GameObject.Find("Crosshair").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(float s, float d)
    {
        speed = s;
        damage = d;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
