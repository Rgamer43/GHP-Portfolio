using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public int id, type;
    public void Activate()
    {
        GameObject.Find("Inventory(Clone)").GetComponent<InventoryScript>().SelectItem(id, type);
    }
}
