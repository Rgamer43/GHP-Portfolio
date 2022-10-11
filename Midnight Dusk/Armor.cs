using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    public float damage;
    public float spread;
    public float reloadSpeed;
    public float moveSpeed;
    public int fireMode;

    public float defense;
    public float shield;
    public float shieldRegen;
    public float hp;

    public Armor()
    {

    }

    public Armor(string n, string d, int lv, float dmg, float sprd, float reload, float move, float def, float shld, float shldrgn, float h) : base(n, d, lv)
    {
        damage = dmg;
        spread = sprd;
        reloadSpeed = reload;
        moveSpeed = move;
        defense = def;
        shield = shld;
        shieldRegen = shldrgn;
        hp = h;
    }

    public Armor(int i) : base(i)
    {
        Armor b = (Armor) ItemList.items[i];

        damage = b.damage;
        spread = b.spread;
        reloadSpeed = b.reloadSpeed;
        moveSpeed = b.moveSpeed;
        defense = b.defense;
        shield = b.shield;
        shieldRegen = b.shieldRegen;
        hp = b.hp;
    }

    public void SetLevel(int l, bool calc)
    {
        int diff = level - l;
        if(diff > 0)
        {
            hp *= Mathf.Pow(Options.HEALTH_SCALING, diff);
            shield *= Mathf.Pow(Options.HEALTH_SCALING, diff);
        }
        else if (diff < 0)
        {
            hp /= Mathf.Pow(Options.HEALTH_SCALING, -diff);
            shield /= Mathf.Pow(Options.HEALTH_SCALING, -diff);
        }

        level = l;
        if(calc && Player.instance.armor == this) Player.instance.CalculateStats();
    }

    public void SetLevel(int l)
    {
        SetLevel(l, true);
    }
}
