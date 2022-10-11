using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade// : MonoBehaviour
{
    public string name;
    public string desc;
    public float effect;
    public int basePrice;
    public int price;
    public float priceScale;
    public int maxLevel;
    public int level;

    public Upgrade(string n, string d, float e, int p, float pS, int mL)
    {
        name = n;
        desc = d;
        effect = e;
        basePrice = p;
        priceScale = pS;
        maxLevel = mL;
    }

    public void CalculatePrice()
    {
        float fPrice = basePrice;

        for(int x = 0; x < level; x++)
        {
            fPrice *= priceScale;
        }

        price = (int)fPrice;
    }
}
