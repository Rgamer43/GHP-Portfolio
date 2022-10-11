using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class Inventory
{
    [SerializeField]
    public List<ItemHolder> items = new List<ItemHolder>();

    public System.Type type;
    public int size;
    public int takenSpace;

    public Inventory(System.Type t, int s)
    {
        type = t;
        size = s;
        takenSpace = 0;
    }

    public Inventory(System.Type t, int s, ItemHolder[] i)
    {
        type = t;
        size = s;
        items = i.ToList();
    }

    public bool HasSpace(int a)
    {
        if (GetTakenSpace() + a <= size || size == -1) return true;
        return false;
    }

    public bool HasSpace()
    {
        return HasSpace(0);
    }

    public int GetTakenSpace()
    {
        takenSpace = 0;
        for (int i = 0; i < items.Count; i++) takenSpace += items[i].amount;
        return takenSpace;
    }

    public ItemHolder GetHolder(Item i)
    {
        for (int x = 0; x < items.Count; x++)
            if (items[x].item.id == i.id && !(i is Weapon)) return items[x];
            else if (items[x].item == i) return items[x];

        return null;
    }

    public int AddItem(Item i, int a)
    {
        return AddItem(i, a, false);
    }

    public int AddItem(Item i, int a, bool newHolder)
    {
        System.Type t = i.GetType();
        if (true)
        {
            if (GetHolder(i) != null)
            {
                if (takenSpace + a <= size || size == -1)
                {
                    int amt = a;
                    if (!newHolder) amt = GetHolder(i).Add(a);
                    else items.Add(new ItemHolder(i, a));
                    if (amt < a) AddItem(i, a - amt, true);
                    GetTakenSpace();
                    return a;
                }
                else
                {
                    int amt = takenSpace - size;
                    GetHolder(i).Add(amt);
                    GetTakenSpace();
                    return amt;
                }
            }
            else
            {
                if (takenSpace + a <= size || size == -1)
                {
                    items.Add(new ItemHolder(i, a));
                    GetTakenSpace();
                    return a;
                }
                else
                {
                    int amt = size - takenSpace;
                    items.Add(new ItemHolder(i, amt));
                    GetTakenSpace();
                    return amt;
                }
            }
        }
        else
        {
            Log.LogError("Item is of the wrong type");
            return 0;
        }
    }

    public int RemoveItem(Item i, int a)
    {
        if(type.IsAssignableFrom(i.GetType()))
        {
            if (GetHolder(i) != null)
            {
                if (takenSpace - a <= size || size == -1)
                {
                    GetHolder(i).Add(-a);
                    GetTakenSpace();
                    if (GetHolder(i).amount < 1) items.Remove(GetHolder(i));
                    return a;
                }
                else
                {
                    int amt = size - takenSpace;
                    GetHolder(i).Add(-amt);
                    GetTakenSpace();
                    if (GetHolder(i).amount < 1) items.Remove(GetHolder(i));
                    return amt;
                }

            }
            else {
                Log.LogWarning("Item holder not found");
                return 0;
            }
        }

        return 0;
    }

    public bool RemoveAt(int i)
    {
        if (items.Count() > 1)
        {
            items.RemoveAt(i);
            return true;
        }

        return false;
    }

    public int Count()
    {
        return items.Count;
    }

    public ItemHolder Get(int i)
    {
        return items[i];
    }
}