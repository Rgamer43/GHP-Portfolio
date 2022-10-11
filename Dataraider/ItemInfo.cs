using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{

    public Item item;

    public Text text;
    public Button button;

    public GameObject weaponSelect;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "Name: " + item.name + "\n" +
                    item.type + "\n" +
                    "Cost: $" + item.cost + "\n";
        if (item is Weapon) {
            Weapon weapon = (Weapon)item;
            text.text += "Damage: " + weapon.damage + "\n" +
            "Fire Rate: " + weapon.speed + "\n" +
            "Spread: " + weapon.spread + "\n" +
            "Reload Speed: " + weapon.reloadSpeed + "\n" +
            "Bullets Per Magazine: " + weapon.ammoPerMag + "\n" +
            "Bullet Speed: " + weapon.bulletSpeed;

            if(weapon.fireType == 2)
            {
                text.text += "\n Burst Size: " + weapon.burstSize + "\n" +
                    "Burst Speed: " + weapon.burstSpeed;
            }

            if(weapon.bulletType == 1)
            {
                text.text += "\n Explosion Size: " + weapon.explosionSize + "\n" +
                    "Explosion Lifetime: " + weapon.explosionLifetime;
            }
        }
        else if(item is Armor)
        {
            Armor armor = (Armor)item;
            text.text += "Health Bonus: " + armor.healthBonus + "\n" +
            "Damage Reduction: " + armor.damageReduction.ToString() + "% \n" +
            "Speed Modifier: " + armor.speedMod + "\n" +
            "Jump Modifier: " + armor.jumpMod;
        }

        text.text += "\n" + item.desc;

        if(GameObject.Find("Game Manager").GetComponent<GameManager>().money < item.cost)
        {
            button.interactable = false;
            button.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Can't Afford";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exit()
    {
        Destroy(gameObject);
        GameObject select = Instantiate(weaponSelect, this.transform.position, this.transform.rotation);
        select.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    public void Purchase()
    {
        if (GameObject.Find("Game Manager").GetComponent<GameManager>().money >= item.cost)
        {
            GameObject.Find("Game Manager").GetComponent<GameManager>().money -= item.cost;
            Destroy(gameObject);

            GameManager gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
            if (gm.uiNumber == 0)
            {
                print(gm.uiNumber);
                print(gm.playerWeapons[0].name + " - " + gm.playerWeapons[1].name);
                print(item.GetType());
                gm.playerWeapons[0] = (Weapon)item;
            }
            else if (gm.uiNumber == 1)
            {
                print(gm.uiNumber);
                print(gm.playerWeapons[0].name + " - " + gm.playerWeapons[1].name);
                print(item.GetType());
                gm.playerWeapons[1] = (Weapon)item;
            }
            else if (gm.uiNumber == 2)
            {
                print(gm.uiNumber);
                print(gm.playerWeapons[0].name + " - " + gm.playerWeapons[1].name);
                print(item.GetType());
                Armor a = (Armor)item;
                print(item.GetType());
                gm.playerArmor = a;
            }

            GameObject.Find("Game Manager").GetComponent<GameManager>().NextScreen();
        }
    }
}
