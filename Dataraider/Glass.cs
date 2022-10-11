using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    //public AudioClip sfx;

    public void OnCollisionEnter(Collision collision)
    {
        print("Glass registered collision");
        if(collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "Rocket")
        {
            print("Breaking glass");
            AudioSource source = GetComponent<AudioSource>();
            if (gameObject.GetComponent<AudioSource>() == null) print("ERROR: No audio source found");
            gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(source.clip);
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            Destroy(gameObject,0.85f);
        }
    }
}
