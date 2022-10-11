using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public readonly static int upgradesNum = 13;

    [SerializeField]
    public int money;
    [SerializeField]
    public int[] upgrades = new int[upgradesNum];

    public Save(int money)
    {
        this.money = money;

        this.upgrades = new int[upgradesNum];

        int[] u = new int[upgradesNum];
        int[] upgrades = new int[upgradesNum];
        for (int x = 0; x < upgrades.Length; x++) u[x] = upgrades[x];
        upgrades = u;

        for (int x = 0; x < upgradesNum; x++)
        {
            this.upgrades[x] = SaveManager.LoadGame(SaveTracker.instance.id).upgrades[x];
        }
    }

    public Save(int money, int[] upgrades)
    {
        this.money = money;

        this.upgrades = new int[upgradesNum];

        int[] u = new int[upgradesNum];
        for (int x = 0; x < upgrades.Length; x++) u[x] = upgrades[x];
        upgrades = u;

        Debug.Log(this.upgrades.Length + ", " + upgrades.Length);

        for (int x = 0; x < upgradesNum; x++)
        {
            this.upgrades[x] = upgrades[x];
        }
    }
}
