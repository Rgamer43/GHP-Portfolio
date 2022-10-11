using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    public Weapon[] weapons;
    [SerializeField]
    public Armor[] armors;

    //Game Start UI
    public int uiNumber;
    public GameObject weapon1SelectUI;
    public GameObject weapon2SelectUI;
    public GameObject armorSelectUI;
    public GameObject readyUI;

    public Transform uiPosition;

    public int money = 0;

    public Weapon[] playerWeapons;
    public Armor playerArmor;

    public GameObject playerModel;

    public GameObject player;

    public float waveLength;
    public int[] enemiesPerWave;
    [SerializeField]
    public EnemyList[] enemyModels;

    public int wave = 1;

    public int enemies;
    public int maxEnemies;

    public int[] pickUpsPerWave;
    public GameObject[] pickUps;

    //Transforms
    public Transform playerSpawn;
    public Transform[] enemySpawns;

    // Start is called before the first frame update
    void Start()
    {
        //print(SaveManager.LoadGame(SaveManager.id).money);
        print(SaveTracker.instance.id);
        money = SaveManager.LoadGame(SaveTracker.instance.id).money;

        uiNumber = 0;
        GameObject ui = Instantiate(weapon1SelectUI, uiPosition.position, uiPosition.rotation);
        ui.transform.SetParent(GameObject.Find("Canvas").transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextScreen()
    {
        if(uiNumber == 0)
        {
            uiNumber++;
            GameObject ui = Instantiate(weapon2SelectUI, uiPosition.position, uiPosition.rotation);
            ui.transform.SetParent(GameObject.Find("Canvas").transform);
        }
        else if(uiNumber == 1)
        {
            uiNumber++;
            GameObject ui = Instantiate(armorSelectUI, uiPosition.position, uiPosition.rotation);
            ui.transform.SetParent(GameObject.Find("Canvas").transform);
        }
        else if(uiNumber == 2)
        {
            uiNumber++;
            GameObject ui = Instantiate(readyUI, uiPosition.position, uiPosition.rotation);
            ui.transform.SetParent(GameObject.Find("Canvas").transform);
        }
    }

    public void StartGame()
    {
        SaveManager.SaveGame(new Save(money), SaveTracker.instance.id);
        Destroy(GameObject.Find("Camera"));
        player = Instantiate(playerModel, enemySpawns[Random.Range(0,enemySpawns.Length)].position, playerSpawn.rotation);
        Player playerScript = player.GetComponent<Player>();

        playerScript.SetGear(playerWeapons, playerArmor);

        InvokeRepeating("NewWave", waveLength/3, waveLength);
    }

    public void NewWave()
    {
        int e = Random.Range(enemiesPerWave[0], enemiesPerWave[1]+((wave*3)-3));

        for(int x = 0; x < e; x++)
        {
            if (enemies < maxEnemies)
            {
                int w = wave / 10;
                w = 0; //Setting it to only use the first tier of enemies
                if (w >= enemyModels.Length) w = enemyModels.Length - 1;
                int y = Random.Range(0, w);
                Transform spawn = enemySpawns[Random.Range(0, enemySpawns.Length)];
                var enemy = Instantiate(enemyModels[w].enemies[Random.Range(0,enemyModels[w].enemies.Length)], spawn.position, spawn.rotation);
                enemy.GetComponent<Enemy>().player = player;

                enemies++;
            }
            else break;
        }

        int p = Random.Range(pickUpsPerWave[0], pickUpsPerWave[1]);

        for(int x = 0; x < p; x++)
        {
            Transform spawn = enemySpawns[Random.Range(0, enemySpawns.Length)];
            var pickUp = Instantiate(pickUps[Random.Range(0, pickUps.Length)], spawn.position, spawn.rotation);
        }

        wave++;
        print("Wave " + wave + " has started!");
    }
}
