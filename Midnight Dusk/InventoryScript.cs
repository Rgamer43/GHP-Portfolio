using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{

    public Text stats, inspect;
    public Player player;
    public GameObject itemTemplate, listHeader, inventoryHeader, prefab;
    public GameObject[] buttons;
    public int[] selected = new int[] { -1, -1 };

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        Inventory inv = player.inventory;

        Vector2 headerPos = inventoryHeader.gameObject.transform.position;

        selected = new int[2];

        GameObject header = Instantiate(listHeader);
        header.transform.SetParent(inventoryHeader.transform);
        header.GetComponent<Text>().text = "Armor:";
        GameObject b = Instantiate(itemTemplate, new Vector2(headerPos.x, inventoryHeader.gameObject.GetComponent<RectTransform>().position.y), Quaternion.identity);
        b.transform.SetParent(inventoryHeader.transform);
        b.name = "armor " + 0;
        b.transform.GetChild(0).GetComponent<Text>().text = player.armor.name;
        b.GetComponent<ItemButton>().id = 0;
        b.GetComponent<ItemButton>().type = 1;

        header = Instantiate(listHeader);
        header.transform.SetParent(inventoryHeader.transform);
        header.GetComponent<Text>().text = "Weapons:";
        for(int i = 0; i < player.weapons.Count(); i++)
        {
            b = Instantiate(itemTemplate, new Vector2(headerPos.x, inventoryHeader.gameObject.GetComponent<RectTransform>().position.y), Quaternion.identity);
            b.transform.SetParent(inventoryHeader.transform);
            b.name = "weapon " + i;
            b.transform.GetChild(0).GetComponent<Text>().text = player.weapons.Get(i).item.name;
            b.GetComponent<ItemButton>().id = i;
            b.GetComponent<ItemButton>().type = 2;
        }

        header = Instantiate(listHeader);
        header.transform.SetParent(inventoryHeader.transform);
        header.GetComponent<Text>().text = "Cybernetics:";
        for (int i = 0; i < player.cybernetics.Count(); i++)
        {
            b = Instantiate(itemTemplate, new Vector2(headerPos.x, inventoryHeader.gameObject.GetComponent<RectTransform>().position.y), Quaternion.identity);
            b.transform.SetParent(inventoryHeader.transform);
            b.name = "cybernetic " + i;
            b.transform.GetChild(0).GetComponent<Text>().text = player.cybernetics.Get(i).item.name;
            b.GetComponent<ItemButton>().id = i;
            b.GetComponent<ItemButton>().type = 3;
        }

        header = Instantiate(listHeader);
        header.transform.SetParent(inventoryHeader.transform);
        header.GetComponent<Text>().text = "Inventory (" + player.inventory.Count() + "/" + player.inventory.size + "):";

        for (int i = 0; i < inv.Count(); i++)
        {
            b = Instantiate(itemTemplate, new Vector2(headerPos.x, inventoryHeader.gameObject.GetComponent<RectTransform>().position.y), Quaternion.identity);
            b.transform.SetParent(inventoryHeader.transform);
            b.name = "item " + i;
            b.transform.GetChild(0).GetComponent<Text>().text = player.inventory.Get(i).item.name + " x" + player.inventory.Get(i).amount;
            b.GetComponent<ItemButton>().id = i;
            b.GetComponent<ItemButton>().type = 0;
        }

        transform.GetChild(0).GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    }

    // Update is called once per frame
    void Update()
    {
        string t = "";

        t += "HP: " + player.hp + "/" + player.maxHP;
        t += "\nShield: " + player.shield + "/" + player.maxShield;
        t += "\nShield Regen: " + player.shieldRegen + "s";
        t += "\nDefense: " + (player.defense * 100) + "%";
        t += "\n\nMove Speed: " + (player.speed * 100 / player.BASE_SPEED) + "%";
        t += "\nDash Force: " + (player.dashForce * 100 / player.BASE_DASH_FORCE) + "%";
        t += "\nDash Cooldown: " + player.dashCooldown + "s";

        stats.text = t;
    }

    public void SelectItem(int id, int type)
    {
        string text = "";
        Item i = null;

        selected = new int[]{ type, id };

        if (type == 0) i = player.inventory.Get(id).item;
        else if (type == 1) i = player.armor;
        else if (type == 2) i = player.weapons.Get(id).item;
        else if (type == 3) i = player.cybernetics.Get(id).item;

        if(type < 1)
            text = i.name + " x" + player.inventory.Get(id).amount;
        else
            text = i.name;

        if(i is Armor) text += "\nArmor - ";
        else if (i is Weapon) text += "\n" + ((Weapon)i).category + " - ";
        else if (i is Cybernetic) text += "\nCybernetic - ";
        else if (i is Modification) text += "\nWeapon Modification - ";
        else if(i is Item) text += "\nItem - ";

        text += "Level " + i.level;
        if (i.lives > 0) text += ", " + i.lives + " Lives";
            else text += ", Unbreakable";
            text += "\n" + i.desc;

        if(i is Armor)
        {
            Armor a = (Armor)i;
            text += "\n";
            text += "\nHP: " + a.hp;
            text += "\nShield: " + a.shield + " (Regens in " + a.shieldRegen + "s)";
            text += "\nDefense: " + (a.defense * 100) + "%";
            if(a.moveSpeed > 0) text += "\nMove Speed: +" + (a.moveSpeed * 100) + "%";
            else if(a.moveSpeed < 0) text += "\nMove Speed: " + (a.moveSpeed * 100) + "%";

            if(type == 0)
            {
                buttons[0].SetActive(true);
                for (int n = 1; n < buttons.Length; n++) buttons[n].gameObject.SetActive(false);

                buttons[0].transform.GetChild(0).GetComponent<Text>().text = "Equip";
            }
            else if (type == 1)
            {
                for (int n = 0; n < player.weapons.Count(); n++) buttons[n].SetActive(false);
                for (int n = player.weapons.Count(); n < buttons.Length; n++) buttons[n].gameObject.SetActive(false);
            }
        } 
        
        else if(i is Weapon)
        {
            Weapon w = (Weapon)i;
            text += "\n";
            text += "\nDeals " + w.damage + " damage at " + w.rpm + " RPM";
            if (w.bulletCount > 1) text += " (Fires " + w.bulletCount + " bullets at once)";
            text += "\nMagazine Size: " + w.magSize + ", " + w.reloadSpeed + "s reload speed";
            text += "\nSpread: " + w.spread + " degrees";
            text += "\nField of View: " + w.fov;
            text += "\nRange: " + w.range[0] + "m/" + w.range[1] + "m";

            //text += "\nRecoil: " + w.recoil;

            text += "\n\nMods (" + w.mods.Count() + "/" + w.mods.size + "): ";
            for (int n = 0; n < w.mods.Count(); n++)
            {
                text += w.mods.Get(n).item.name;
                if (n < w.mods.Count() - 1) text += ", ";
            }

            if (type == 0)
            {
                for (int n = 0; n < player.weapons.size; n++) buttons[n].SetActive(true);
                for (int n = player.weapons.size; n < buttons.Length; n++) buttons[n].gameObject.SetActive(false);

                for(int n = 0; n < player.weapons.size; n++)
                    buttons[n].transform.GetChild(0).GetComponent<Text>().text = "Equip in slot " + (n+1);
            } else if(type == 2 && player.weapons.size > 1)
            {
                for (int n = 1; n < buttons.Length; n++) buttons[n].gameObject.SetActive(false);
                buttons[0].SetActive(true);
                buttons[0].transform.GetChild(0).GetComponent<Text>().text = "Dequip";
            }
        }

        else if(i is Cybernetic)
        {
            Cybernetic c = (Cybernetic)i;
            text += "\n";

            if (c.hp > 0) text += "\n+" + (c.hp * 100) + "% HP";
            else if (c.hp < 0) text += "\n" + (c.hp * 100) + "% HP";

            if (c.defense > 0) text += "\n+" + (c.defense * 100) + "% defense";
            else if (c.defense < 0) text += "\n" + (c.defense * 100) + "% defense";

            if (c.shield > 0) text += "\n+" + (c.shield * 100) + "% shield";
            else if (c.shield < 0) text += "\n" + (c.shield * 100) + "% shield";

            if (c.shieldRegen > 0) text += "\n+" + (c.shieldRegen * 100) + "% shieldRegen";
            else if (c.shieldRegen < 0) text += "\n" + (c.shieldRegen * 100) + "% shieldRegen";

            if (c.moveSpeed > 0) text += "\n+" + (c.moveSpeed * 100) + "% moveSpeed";
            else if (c.moveSpeed < 0) text += "\n" + (c.moveSpeed * 100) + "% moveSpeed";

            if (c.rpm > 0) text += "\n+" + (c.rpm * 100) + "% weapon RPM";
            else if (c.rpm < 0) text += "\n" + (c.rpm * 100) + "% weapon RPM";

            if (c.spread > 0) text += "\n+" + (c.spread * 100) + "% weapon spread";
            else if (c.spread < 0) text += "\n" + (c.spread * 100) + "% weapon spread";

            if (c.damage > 0) text += "\n+" + (c.damage * 100) + "% weapon damage";
            else if (c.damage < 0) text += "\n" + (c.damage * 100) + "% weapon damage";

            if (c.fov > 0) text += "\n+" + (c.fov * 100) + "% FOV";
            else if (c.fov < 0) text += "\n" + (c.fov * 100) + "% FOV";

            if (c.recoil > 0) text += "\n+" + (c.recoil * 100) + "% weapon recoil";
            else if (c.recoil < 0) text += "\n" + (c.recoil * 100) + "% weapon recoil";

            if (c.reloadSpeed > 0) text += "\n+" + (c.reloadSpeed * 100) + "% weapon reload speed";
            else if (c.reloadSpeed < 0) text += "\n" + (c.reloadSpeed * 100) + "% weapon reload speed";

            if (c.range[0] > 0) text += "\n+" + (c.range[0] * 100) + "% weapon damage dropoff range";
            else if (c.range[0] < 0) text += "\n" + (c.range[0] * 100) + "% weapon damage dropoff range";

            if (c.range[1] > 0) text += "\n+" + (c.range[1] * 100) + "% max range";
            else if (c.range[1] < 0) text += "\n" + (c.range[1] * 100) + "% max range";
        }

        else if (i is Modification)
        {
            Modification m = (Modification)i;
            text += "\n";

            if (m.rpm > 0) text += "\n+" + (m.rpm * 100) + "% RPM";
            else if (m.rpm < 0) text += "\n" + (m.rpm * 100) + "% RPM";

            if (m.spread > 0) text += "\n+" + (m.spread * 100) + "% spread";
            else if (m.spread < 0) text += "\n" + (m.spread * 100) + "% spread";

            if (m.damage > 0) text += "\n+" + (m.damage * 100) + "% damage";
            else if (m.damage < 0) text += "\n" + (m.damage * 100) + "% damage";

            if (m.fov > 0) text += "\n+" + (m.fov * 100) + "% FOV";
            else if (m.fov < 0) text += "\n" + (m.fov * 100) + "% FOV";

            if (m.recoil > 0) text += "\n+" + (m.recoil * 100) + "% recoil";
            else if (m.recoil < 0) text += "\n" + (m.recoil * 100) + "% recoil";

            if (m.magSize > 0) text += "\n+" + (m.magSize * 100) + "% magazine size";
            else if (m.magSize < 0) text += "\n" + (m.magSize * 100) + "% magazine size";

            if (m.reloadSpeed > 0) text += "\n+" + (m.reloadSpeed * 100) + "% reload speed";
            else if (m.reloadSpeed < 0) text += "\n" + (m.reloadSpeed * 100) + "% reload speed";

            if (m.range[0] > 0) text += "\n+" + (m.range[0] * 100) + "% damage dropoff range";
            else if (m.range[0] < 0) text += "\n" + (m.range[0] * 100) + "% damage dropoff range";

            if (m.range[1] > 0) text += "\n+" + (m.range[1] * 100) + "% max range";
            else if (m.range[1] < 0) text += "\n" + (m.range[1] * 100) + "% max range";

            if (m.velocity > 0) text += "\n+" + (m.velocity * 100) + "% bullet velocity";
            else if (m.velocity < 0) text += "\n" + (m.velocity * 100) + "% bullet velocity";
        }

        inspect.text = text;
    }

    public void Button(int i)
    {
        if(selected[0] == 2)
        {
            player.inventory.AddItem(player.weapons.Get(selected[1]).item, 1);
            Log.LogMsg("Removed " + player.weapons.RemoveItem(player.weapons.Get(selected[1]).item, 1));
            Instantiate(Resources.Load<GameObject>("Prefabs/Inventory"), transform.position, Quaternion.identity);
            Destroy(gameObject);
            player.heldWeapon = 0;
        }
        
        else if (selected[0] == 0)
        {
            if (player.inventory.Get(selected[1]).item is Armor)
            {
                player.inventory.AddItem(player.armor, 1);
                player.armor = (Armor)player.inventory.Get(selected[1]).item;
                player.inventory.RemoveItem(player.armor, 1);
                Instantiate(Resources.Load<GameObject>("Prefabs/Inventory"), transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

            if(player.inventory.Get(selected[1]).item is Weapon)
            {
                if (player.weapons.Count() > i)
                {
                    player.inventory.AddItem(player.weapons.Get(i).item, 1);
                    player.weapons.Get(i).item = (Weapon)player.inventory.Get(selected[1]).item;
                    player.SetHeldWeapon(player.heldWeapon);
                }
                else player.weapons.AddItem(player.inventory.Get(selected[1]).item, 1);

                player.inventory.RemoveItem(player.inventory.Get(selected[1]).item, 1);
                Instantiate(Resources.Load<GameObject>("Prefabs/Inventory"), transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        player.CalculateStats();
        SaveManager.Save();
    }
}
