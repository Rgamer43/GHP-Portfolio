using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modification : Item
{

    public float damage;
    public float rpm;
    public float spread;
    public float magSize;
    public float reloadSpeed;
    public float[] range;
    public float velocity;
    public float fov;
    public float recoil;

    public Modification()
    {

    }

    public Modification(string n, string d, int lv, float dmg, float rpm, float sprd, float mag, float reload, float[] r, float vel, float fov, float rcl) : base(n, d, lv)
    {
        damage = dmg;
        this.rpm = rpm;
        spread = sprd;
        magSize = mag;
        reloadSpeed = reload;
        range = r;
        velocity = vel;
        this.fov = fov;
        recoil = rcl;
    }

    public Modification(int i) : base(i)
    {
        Modification b = (Modification) ItemList.items[i];

        damage = b.damage;
        rpm = b.rpm;
        spread = b.spread;
        magSize = b.magSize;
        reloadSpeed = b.reloadSpeed;
        range = b.range;
        velocity = b.velocity;
        fov = b.fov;
        recoil = b.recoil;
    }
}
