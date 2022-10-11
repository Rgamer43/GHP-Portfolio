using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cybernetic : Item
{

    public float damage;
    public float rpm;
    public float spread;
    public float reloadSpeed;
    public float moveSpeed;
    public float[] range;
    public float fov;
    public float recoil;

    public float defense;
    public float shield;
    public float shieldRegen;
    public float hp;

    public Cybernetic()
    {
        
    }

    public Cybernetic(string n, string d, int lv, float dmg, float rpm, float sprd, float reload, float move, float[] r, float def, float shld, float h, float shldrgn, float fov, float rcl) : base(n, d, lv)
    {
        damage = dmg;
        this.rpm = rpm;
        spread = sprd;
        reloadSpeed = reload;
        moveSpeed = move;
        range = r;
        defense = def;
        shield = shld;
        shieldRegen = shldrgn;
        hp = h;
        this.fov = fov;
        recoil = rcl;
    }

    public Cybernetic(int i) : base(i)
    {
        Cybernetic b = (Cybernetic) ItemList.items[i];

        damage = b.damage;
        rpm = b.rpm;
        spread = b.spread;
        reloadSpeed = b.reloadSpeed;
        moveSpeed = b.moveSpeed;
        range = b.range;
        defense = b.defense;
        shield = b.shield;
        shieldRegen = b.shieldRegen;
        hp = b.hp;
        fov = b.fov;
        recoil = b.recoil;
    }
}
