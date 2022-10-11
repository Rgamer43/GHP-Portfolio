using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemHolder
{
    [SerializeField]
    public Item item;
    public int amount;
    public int max;

    public ItemHolder(Item i, int a, int m)
    {
        item = i;
        amount = a;
        max = m;

        if (item is Weapon) max = 1;
    }

    public ItemHolder(Item i, int a)
    {
        item = i;
        amount = a;
        max = -1;

        if (item is Weapon) max = 1;
    }

    public int Add(int a)
    {
        int amt = a;

        if (amt + amount > max && max != -1) amt = max - amount;

        amount += a;
        if (amount > max && max != -1)
        {
            amount = max;
            return amt;
        }

        return a;
    }

    public bool HasSpace(int a)
    {
        if (amount + a > max && max != -1) return false;
        return true;
    }

    public Modification GetMod()
    {
        if (item.GetType() == new Modification().GetType()) return (Modification)item;
        else return null;
    }

    public Weapon GetWeapon()
    {
        if (item.GetType() == new Weapon().GetType()) return (Weapon)item;
        else return null;
    }

    public Cybernetic GetCybernetic()
    {
        if (item.GetType() == new Cybernetic().GetType()) return (Cybernetic)item;
        else return null;
    }

    public Armor GetArmor()
    {
        if (item.GetType() == new Armor().GetType()) return (Armor)item;
        else return null;
    }
}
