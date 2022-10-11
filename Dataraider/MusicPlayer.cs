using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public AudioSource audioSource;

    public AudioClip[] songs;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying) 
        {
            int x = Random.Range(0, songs.Length);
            audioSource.clip = songs[x];
            audioSource.Play();
            print("Playing song " + x);
        }
    }
}
