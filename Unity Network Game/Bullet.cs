using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{

    public int dmg;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc]
    public void FireServerRPC(int d, float force)
    {
        print("Firing bullet...");
        if (IsServer)
        {
            print("Firing bullet on server...");
            dmg = d;
            GetComponent<Rigidbody2D>().AddForce(transform.forward * force);
            Destroy(gameObject, 5f);
            NetworkManager.Destroy(gameObject, 5f);
            Invoke("CleanUp", 5f);
        }
    }

    public void CleanUp()
    {
        Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
