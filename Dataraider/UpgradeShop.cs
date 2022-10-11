using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{

    public Text text;
    public Text moneyText;
    public Button button;

    public int saveID;

    public Upgrade[] upgrades;
    public int[] levels;
    public int money;

    public int currentID = 0;

    // Start is called before the first frame update
    void Start()
    {
        saveID = SaveTracker.instance.id;
        levels = SaveManager.LoadGame(saveID).upgrades;
        upgrades = UpgradeList.upgrades;
        money = SaveManager.LoadGame(saveID).money;

        ViewUpgrade(currentID);

        print(Save.upgradesNum + ", " + SaveManager.LoadGame(SaveTracker.instance.id).upgrades.Length);

        for (int x = 0; x < upgrades.Length; x++)
        {
            print(upgrades[x].name);
            print(levels[x]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        moneyText.text = "Money: " + money;

        upgrades[currentID].CalculatePrice();

        if (upgrades[currentID].maxLevel == levels[currentID])
        {
            button.interactable = false;
            button.transform.GetChild(0).GetComponent<Text>().text = "Max Level";
        }
        else if (upgrades[currentID].price > money)
        {
            button.interactable = false;
            button.transform.GetChild(0).GetComponent<Text>().text = "Can't Afford";
        }
        else
        {
            button.interactable = true;
            button.transform.GetChild(0).GetComponent<Text>().text = "Purchase";
        }
    }

    public void ViewUpgrade(int id)
    {
        upgrades[id].CalculatePrice();

        text.text = upgrades[id].name + " " + levels[id] + "\n\n"
            + upgrades[id].desc + "\n";
        if(levels[id] < upgrades[id].maxLevel) text.text += "Price: " + upgrades[id].price;

       if(upgrades[id].maxLevel == levels[id])
       {
           button.interactable = false;
           button.transform.GetChild(0).GetComponent<Text>().text = "Max Level";
       }
       else if (upgrades[id].price <= money)
       {
           button.interactable = false;
           button.transform.GetChild(0).GetComponent<Text>().text = "Can't Afford";
       }
       else
       {
           button.interactable = true;
           button.transform.GetChild(0).GetComponent<Text>().text = "Purchase";
       }
    }

    public void ViewRight()
    {
        currentID++;
        if (currentID >= levels.Length) currentID = 0;
        ViewUpgrade(currentID);
    }

    public void ViewLeft()
    {
        currentID--;
        if (currentID < 0) currentID = levels.Length-1;
        ViewUpgrade(currentID);
    }

    public void Purchase()
    {
        upgrades[currentID].CalculatePrice();

        money -= upgrades[currentID].price;
        levels[currentID] += 1;
        upgrades[currentID].level += 1;

        SaveManager.SaveGame(new Save(money, levels), saveID);

        ViewUpgrade(currentID);
    }
}
