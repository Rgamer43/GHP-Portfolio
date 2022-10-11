using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public string name;
    public int level;
    public string desc;
    public int lives;

    public int id;

    public Item()
    {

    }

    public Item(string n, string d, int lv)
    {
        name = n;
        level = 1;
        desc = d;
        lives = lv;
    }

    public Item(int i)
    {
        id = i;
        Item b = ItemList.items[i];
        name = b.name;
        level = b.level;
        desc = b.desc;
        lives = b.lives;
    }

    public int AddLives(int a)
    {
        if (lives == -1) return lives;
        lives += a;
        return lives;
    }

    public void SetLevel(int l)
    {
        level = l;
    }
}
