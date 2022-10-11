using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : Item
{

    public GameObject model;
    public GameObject bullet;
    public Transform bulletSpawn;
    
    public float damage;
    public float speed;
    public float fireType; //0 is semi, 1 is auto, 2 is burst
    public int bulletType; //0 is bullet, 1 is rocket, 2 is grenade
    public float bulletSpeed;
    public float spread;
    public int ammoPerMag;
    public float reloadSpeed;
    public int burstSize;
    public float burstSpeed;
    public bool playBurstSound;
    public bool consumeBurstAmmo;
    public float explosionSize;
    public float explosionLifetime;
    public bool useAmmo;
    public bool infiniteAmmo;
    public bool spawnFX;
    public float speedMod;
    public float jumpMod;

    public int ammo;
    public int unloadedAmmo;

    public AudioClip shotSound;

}
