using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class SaveManager
{

    public static List<string> saves = new List<string>();

    public static string currentSave;

    public static string[] GetSaves()
    {
        string filePath = Application.persistentDataPath;

        if (!File.Exists(filePath + "/saves")) return new string[0];

        string[] text = System.IO.File.ReadAllLines(filePath + "/saves");
        saves = text.ToList();

        if (text.Length == 0) return new string[0];
        return text;
    }

    public static void DeleteSave(string save)
    {
        saves.Remove(save);
        File.WriteAllLines(Application.persistentDataPath + "/saves", saves);
        File.Delete(Application.persistentDataPath + "/" + save + ".save");
    }

    public static void CreateSave(string save)
    {
        Debug.Log("Creating save...");
        File.Create(Application.persistentDataPath + "/" + save + ".save");
        saves.Add(save);
        File.WriteAllLines(Application.persistentDataPath + "/saves", saves);
    }

    public static bool IsNewGame()
    {
        //Debug.Log(Application.persistentDataPath + "/" + currentSave + ".save");
        //Debug.Log(File.ReadAllLines(Application.persistentDataPath + "/" + currentSave + ".save").Length);
        return File.ReadAllLines(Application.persistentDataPath + "/" + currentSave + ".save").Length == 0;
    }

    public static void Save()
    {
        Log.LogImportant("Saving...");

        Player player = Player.instance;
        string save = "";

        //Armor
        Log.LogMsg("Saving armor...");
        save += player.armor.id + " " + player.armor.level + " " + player.armor.lives;

        //Weapons
        Log.LogMsg("Saving weapons...");
        save += "\n/e/";
        for (int i = 0; i < player.weapons.Count(); i++)
        {
            Weapon item = (Weapon)player.weapons.Get(i).item;
            save += "\n" + player.weapons.Get(i).item.id;
            save += " " + item.level + " " + item.lives;
            for (int n = 0; n < ((Weapon)player.weapons.Get(i).item).mods.Count(); n++) {
                save += " " + ((Weapon)player.weapons.Get(i).item).mods.Get(n).item.id + " " ;
            }
        }

        //Cybernetics
        Log.LogMsg("Saving cybernetics...");
        save += "\n/e/";
        for (int i = 0; i < player.cybernetics.Count(); i++)
            save += "\n" + player.cybernetics.Get(i).item.id + " " + player.cybernetics.Get(i).item.lives;

        //Inventory
        Log.LogMsg("Saving inventory...");
        save += "\n/e/";
        for (int i = 0; i < player.inventory.Count(); i++)
        {
            if (player.inventory.Get(i).item is Weapon)
            {
                save += "\nw " + player.inventory.Get(i).item.id + " " + player.inventory.Get(i).amount + " " + player.inventory.Get(i).item.level + " " + player.inventory.Get(i).item.lives;
                Weapon w = (Weapon)player.inventory.Get(i).item;
                for (int n = 0; n < w.mods.Count(); n++)
                {
                    save += " " + w.mods.Get(n).item.id + " ";
                }
            }
            else if (player.inventory.Get(i).item is Armor) save += "\na " + player.inventory.Get(i).item.id + " " + player.inventory.Get(i).amount + " " + player.inventory.Get(i).item.level + " " + player.inventory.Get(i).item.lives;
            else if (player.inventory.Get(i).item is Cybernetic) save += "\nc " + player.inventory.Get(i).item.id + " " + player.inventory.Get(i).amount + " " + player.inventory.Get(i).item.lives;
            else save += "\ni " + player.inventory.Get(i).item.id + " " + player.inventory.Get(i).amount + " " + player.inventory.Get(i).item.lives;
        }

        Log.LogMsg("Writing to file...");
        File.WriteAllText(Application.persistentDataPath + "/" + currentSave + ".save", save);

        Log.LogImportant("Finished saving");
    }

    public static void Load()
    {
        Log.LogImportant("Loading...");

        Player player = GameObject.Find("Player").GetComponent<Player>();
        string[] save = File.ReadAllLines(Application.persistentDataPath + "/" + currentSave + ".save");
        int index = 0;

        Log.LogMsg("Loading armor...");
        player.armor = new Armor(int.Parse(save[index].Split(' ')[0]));
        player.armor.SetLevel(int.Parse(save[index].Split(' ')[1]), false);
        player.armor.lives = int.Parse(save[index].Split(' ')[2]);
        index++;

        if (save[index] == "/e/") index++;

        Log.LogMsg("Loading weapons...");
        player.weapons = new Inventory(new Weapon().GetType(), 2);
        for (; save[index] != "/e/"; index++)
        {
            string[] m = save[index].Split(' ');
            Log.LogMsg(m.Length.ToString());
            Log.LogMsg(save[index]);

            if (m.Length > 2)
            {
                Weapon w = new Weapon(int.Parse(m[0]));
                w.SetLevel(int.Parse(m[1]));
                w.lives = int.Parse(m[2]);
                player.weapons.AddItem(w, 1);
                for (int i = 3; i < m.Length; i++) ((Weapon)player.weapons.Get(player.weapons.Count() - 1).item).mods.AddItem(new Modification(int.Parse(m[i])), 1);
                ((Weapon)player.weapons.Get(player.weapons.Count() - 1).item).ApplyMods();
            }
            else
            {
                Weapon w = new Weapon(int.Parse(m[0]));
                if(m.Length > 2) w.SetLevel(int.Parse(m[2]));
                if (m.Length > 3) w.lives = int.Parse(m[3]);
                player.weapons.AddItem(w, 1);
                Log.LogMsg(player.weapons.Count() + " weapons");
            }
        }

        if (save[index] == "/e/") index++;

        Log.LogMsg("Loading cybernetics...");
        player.cybernetics = new Inventory(new Cybernetic().GetType(), 3);
        for (; save[index] != "/e/"; index++)
        {
            Cybernetic c = new Cybernetic(int.Parse(save[index].Split(' ')[0]));
            c.lives = int.Parse(save[index].Split(' ')[1]);
            player.cybernetics.AddItem(c, 1);
        }

        if (save[index] == "/e/") index++;

        Log.LogMsg("Loading inventory...");
        player.inventory = new Inventory(new Item().GetType(), player.BASE_INVENTORY_SIZE);
        for (; index < save.Length && save[index] != "/e/"; index++)
        {
            string[] m = save[index].Split(' ');

            if (m.Length > 2)
            {
                if (m[0] == "i") player.inventory.AddItem(new Item(int.Parse(m[1])), int.Parse(m[2]));
                else if (m[0] == "w")
                {
                    Weapon w = new Weapon(int.Parse(m[1]));
                    w.SetLevel(int.Parse(m[3]));
                    w.lives = int.Parse(m[4]);

                    for (int n = 5; n < w.mods.Count() && n < m.Length; n++) w.mods.AddItem(new Modification(int.Parse(m[n])), 1);
                    w.ApplyMods();

                    player.inventory.AddItem(w, int.Parse(m[2]));
                }
                else if (m[0] == "a")
                {
                    Armor a = new Armor(int.Parse(m[1]));
                    a.SetLevel(int.Parse(m[3]), false);
                    a.lives = int.Parse(m[4]);
                    player.inventory.AddItem(a, int.Parse(m[2]));
                }
                else if (m[0] == "c")
                {
                    Cybernetic c = new Cybernetic(int.Parse(m[1]));
                    c.lives = int.Parse(m[3]);
                    player.inventory.AddItem(c, int.Parse(m[2]));
                }
            } else
            {
                if(ItemList.items[int.Parse(m[0])] is Weapon) player.inventory.AddItem(new Weapon(int.Parse(m[0])), int.Parse(m[1]));
                else if (ItemList.items[int.Parse(m[0])] is Armor) player.inventory.AddItem(new Armor(int.Parse(m[0])), int.Parse(m[1]));
                else if (ItemList.items[int.Parse(m[0])] is Cybernetic) player.inventory.AddItem(new Cybernetic(int.Parse(m[0])), int.Parse(m[1]));
                else  player.inventory.AddItem(new Item(int.Parse(m[0])), int.Parse(m[1]));
            }
        }

        player.SetHeldWeapon(0);

        player.CalculateStats();
        player.hp = player.maxHP;
        player.shield = player.maxShield;
        
        Log.LogImportant("Finished loading");
    }
}
