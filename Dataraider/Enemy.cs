using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public NavMeshAgent nav;
    public GameObject player;
    public float dist;

    public float range;
    public float hp;
    public float damage;
    public float fireSpeed;
    public GameObject bullet;
    public float bulletSpeed;
    public Transform bulletSpawn;
    public GameObject turret;
    public int ammoPerMag;
    public int ammo;
    public float reloadSpeed;
    public bool isReloading = false;
    public int data;

    // Start is called before the first frame update
    void Start()
    {
        nav = gameObject.GetComponent<NavMeshAgent>();
        InvokeRepeating("Fire", fireSpeed, fireSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            dist = Vector3.Distance(gameObject.transform.position, player.transform.position);
            //if(GameObject.FindGameObjectWithTag("Player") != null) player = GameObject.FindGameObjectWithTag("Player");

            if (dist >= range)
            {
                nav.SetDestination(player.transform.position);
            }
            else
            {
                nav.SetDestination(gameObject.transform.position);
            }

            Transform t = player.transform;

            turret.transform.LookAt(new Vector3(t.position.x, t.position.y + 0.75f, t.position.z));
        }
    }

    public void Fire()
    {
        if(player != null && ammo > 0)
        {
            var myBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
            myBullet.gameObject.GetComponent<Bullet>().Fire(bulletSpeed, damage);
            gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            gameObject.GetComponent<AudioSource>().Play();
            ammo--;
        }
        else if(ammo == 0 && !isReloading)
        {
            Invoke("Reload", reloadSpeed);
            isReloading = true;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet") if(!collision.gameObject.GetComponent<Bullet>().damagePlayer)
        {
            hp -= collision.gameObject.GetComponent<Bullet>().damage;
            Destroy(collision.gameObject);
            
            if(hp <= 0)
            {
                Destroy(gameObject);
                GameObject.Find("Game Manager").GetComponent<GameManager>().enemies--;
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().data += data;
            }

                var hitMarker = Instantiate(collision.gameObject.GetComponent<Bullet>().hitMarker,
                    collision.gameObject.GetComponent<Bullet>().crosshairPos.position, Quaternion.identity);
                Destroy(hitMarker, 0.25f);
        }
    }

    public void Reload()
    {
        ammo = ammoPerMag;
        isReloading = false;
    }
}
