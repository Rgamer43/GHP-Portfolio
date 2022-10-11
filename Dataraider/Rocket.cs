using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifetime;
    protected Rigidbody rb;
    public GameObject explosion;
    public float eSize;
    public float eLifetime;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public void Launch(float s, float d, float e, float l)
    {
        print("Launching Rocket");
        speed = s;
        damage = d;
        eSize = e;
        eLifetime = l;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        print("Rocket has collided");
        GameObject e = Instantiate(explosion, this.transform.position, Quaternion.identity);
        Destroy(e, eLifetime);

        for (float x = 0; x < eSize; x += 0.01f)
        {
            e.transform.localScale = new Vector3(x, x, x);
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int x = 0; x < enemies.Length; x++)
        {
            if (Vector3.Distance(enemies[x].transform.position, gameObject.transform.position) <= eSize)
            {
                enemies[x].GetComponent<Enemy>().hp -= damage;
                if (enemies[x].GetComponent<Enemy>().hp <= 0)
                {
                    Destroy(enemies[x]);
                    GameObject.Find("Game Manager").GetComponent<GameManager>().enemies--;
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().data += enemies[x].GetComponent<Enemy>().data;
                }
            }
        }

        Destroy(gameObject);
    }
}
