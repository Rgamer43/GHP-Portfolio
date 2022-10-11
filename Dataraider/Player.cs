using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour
{

    public static Player instance;

    public Weapon[] weapon;
    public Armor armor;
    public int heldWeapon = 0;

    public GameObject weaponModel;

    public GameObject shootEffect;

    public Transform[] firePos;
    public int isAiming;

    public bool isReloading = false;

    public bool canFire = true;

    public float maxHP;
    public float hp;

    public int data = 0;
    public int moneyPerData = 10;
    public float autoHackSpeed = 10f;

    public int gearCost;

    public GameObject deathObject;

    public float[] fov = new float[2];

    public float[] walkSpeed = new float[2];
    public float[] runSpeed = new float[2];

    public Recoil recoil;

    //UIs
    public Text hpText;
    public Text ammoText;
    public Text dataText;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        EnableFire();
    }

    // Update is called once per frame
    void Update()
    {
        weapon[heldWeapon].bulletSpawn = GameObject.Find("Bullet Spawn").transform;

        if (weaponModel == null) weaponModel = Instantiate(weapon[heldWeapon].model, firePos[isAiming].position, Quaternion.identity);
        weaponModel.transform.SetParent(gameObject.transform.GetChild(0));

        if ((Input.GetMouseButtonDown(0) && (weapon[heldWeapon].fireType == 0 || weapon[heldWeapon].fireType == 2)) || (Input.GetMouseButton(0) && weapon[heldWeapon].fireType == 1))
        {
            if(weapon[heldWeapon].ammo > 0 && canFire)
            {
                Weapon w = weapon[heldWeapon];

                GameObject bullet = Instantiate(weapon[heldWeapon].bullet,
                    weapon[heldWeapon].bulletSpawn.position, weapon[heldWeapon].bulletSpawn.rotation);

                bullet.transform.Rotate(Random.Range(-w.spread, w.spread), Random.Range(-w.spread, w.spread), Random.Range(-w.spread, w.spread));

                if (w.bulletType == 0)
                {
                    print("Firing Bullet");
                    Bullet bScript = bullet.GetComponent<Bullet>();
                    bScript.Fire(weapon[heldWeapon].bulletSpeed, weapon[heldWeapon].damage);
                }
                else if(w.bulletType == 1)
                {
                    print("Firing Rocket");
                    print(bullet.name);
                    Rocket rScript = bullet.GetComponent<Rocket>();
                    rScript.Launch(weapon[heldWeapon].bulletSpeed, weapon[heldWeapon].damage, 
                        weapon[heldWeapon].explosionSize, weapon[heldWeapon].explosionLifetime);
                }

                if(w.useAmmo) weapon[heldWeapon].ammo -= 1;
                canFire = false;

                if (w.spawnFX)
                {
                    GameObject shootFX = Instantiate(shootEffect,
                        weapon[heldWeapon].bulletSpawn.position, weapon[heldWeapon].bulletSpawn.rotation);
                }

                gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                gameObject.GetComponent<AudioSource>().PlayOneShot(weapon[heldWeapon].shotSound);

                if(weapon[heldWeapon].fireType == 2)
                {
                    for(int x = 1; x < weapon[heldWeapon].burstSize; x++)
                    {
                        Invoke("Fire", weapon[heldWeapon].burstSpeed * (float)x);
                    }
                }

                recoil.addRecoil();
            }
            else if(weapon[heldWeapon].unloadedAmmo >= weapon[heldWeapon].ammoPerMag && !isReloading && weapon[heldWeapon].ammo <= 0)
            {
                isReloading = true;
                Invoke("ReloadWeapon", weapon[heldWeapon].reloadSpeed);
            }
        }
        else if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            isReloading = true;
            weapon[heldWeapon].unloadedAmmo += weapon[heldWeapon].ammo;
            weapon[heldWeapon].ammo = 0;
            Invoke("ReloadWeapon", weapon[heldWeapon].reloadSpeed);
        }

        if (Input.GetButtonDown("1"))
        {
            if (heldWeapon != 0)
            {
                Destroy(weaponModel);
                weaponModel = Instantiate(weapon[0].model, firePos[isAiming].position, Quaternion.identity);
                weaponModel.transform.SetParent(gameObject.transform.GetChild(0));
            }
            heldWeapon = 0;

            recoil = weaponModel.GetComponent<Recoil>();
        }
        else if (Input.GetButtonDown("2"))
        {
            if (heldWeapon != 1)
            {
                Destroy(weaponModel);
                weaponModel = Instantiate(weapon[1].model, firePos[isAiming].position, Quaternion.identity);
                weaponModel.transform.SetParent(gameObject.transform.GetChild(0));
            }
            heldWeapon = 1;

            recoil = weaponModel.GetComponent<Recoil>();
        }

        if (Input.GetMouseButton(1)) isAiming = 1;
        else isAiming = 0;

        if(isAiming == 1)
        {
            gameObject.transform.GetChild(0).GetComponent<Camera>().fieldOfView = fov[1];
            GetComponent<FirstPersonController>().m_WalkSpeed = walkSpeed[1];
            GetComponent<FirstPersonController>().m_RunSpeed = runSpeed[1];
        }
        else
        {
            gameObject.transform.GetChild(0).GetComponent<Camera>().fieldOfView = fov[0];
            GetComponent<FirstPersonController>().m_WalkSpeed = walkSpeed[0];
            GetComponent<FirstPersonController>().m_RunSpeed = runSpeed[0];
        }

        weaponModel.transform.position = firePos[isAiming].position;
        weaponModel.transform.rotation = firePos[isAiming].rotation;

        UpdateUI();
    }

    public float TakeDamage(float damage)
    {
        damage *= (100-armor.damageReduction)/100;
        hp -= damage;
        if (hp <= 0) GameOver();
        return damage;
    }

    public float Heal(float healing)
    {
        hp += healing;
        if (hp > maxHP) hp = maxHP;
        return healing;
    }

    public void SetGear(Weapon[] w, Armor a)
    {
        weapon[0] = w[0];
        weapon[0].ammo = weapon[0].ammoPerMag;
        weapon[0].unloadedAmmo = weapon[0].ammoPerMag * 2;
        weapon[1] = w[1];
        weapon[1].ammo = weapon[1].ammoPerMag;
        weapon[1].unloadedAmmo = weapon[1].ammoPerMag * 2;
        armor = a;
        maxHP = 100 + armor.healthBonus;
        hp = maxHP;
        GetComponent<FirstPersonController>().m_JumpSpeed += a.jumpMod + weapon[0].jumpMod + weapon[1].jumpMod;
        GetComponent<FirstPersonController>().m_RunSpeed += a.speedMod + weapon[0].speedMod + weapon[1].speedMod;
        GetComponent<FirstPersonController>().m_WalkSpeed += a.speedMod + weapon[0].speedMod + weapon[1].speedMod;

        ApplyUpgrades();

        walkSpeed = new float[2];
        walkSpeed[0] = GetComponent<FirstPersonController>().m_WalkSpeed;
        walkSpeed[1] = GetComponent<FirstPersonController>().m_WalkSpeed/2;

        runSpeed = new float[2];
        runSpeed[0] = GetComponent<FirstPersonController>().m_RunSpeed;
        runSpeed[1] = GetComponent<FirstPersonController>().m_RunSpeed/2;

        weaponModel = Instantiate(weapon[heldWeapon].model, firePos[isAiming].position, firePos[isAiming].rotation);

        gearCost += w[0].cost;
        gearCost += w[1].cost;
        gearCost += a.cost;

        recoil = weaponModel.GetComponent<Recoil>();
    }

    public void EnableFire()
    {
        canFire = true;
        Invoke("EnableFire", weapon[heldWeapon].speed);
    }

    public void ReloadWeapon()
    {
        isReloading = false;
        if (weapon[heldWeapon].unloadedAmmo >= weapon[heldWeapon].ammoPerMag)
        {
            if(!weapon[heldWeapon].infiniteAmmo) weapon[heldWeapon].unloadedAmmo -= weapon[heldWeapon].ammoPerMag;
            weapon[heldWeapon].ammo += weapon[heldWeapon].ammoPerMag;
        }
        else
        {
            weapon[heldWeapon].ammo += weapon[heldWeapon].unloadedAmmo;
            weapon[heldWeapon].unloadedAmmo = 0;
        }
    }

    public void StealData()
    {
        if (hp > 0) data++;
    }

    public void UpdateUI()
    {
        int hpInt = (int)hp;
        hpText.text = "Health: " + hpInt + "/" + maxHP;
        if(!isReloading) ammoText.text = "Ammo: " + weapon[heldWeapon].ammo + "/" + weapon[heldWeapon].unloadedAmmo;
        else ammoText.text = "Ammo: " + "--" + "/" + weapon[heldWeapon].unloadedAmmo;
        dataText.text = "Data Stolen: " + data + "GB, Wave: " + GameObject.Find("Game Manager").GetComponent<GameManager>().wave;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet") if(collision.gameObject.GetComponent<Bullet>().damagePlayer)
        {
            TakeDamage(collision.gameObject.GetComponent<Bullet>().damage);
            Destroy(collision.gameObject);
        }
    }

    public void GameOver()
    {
        Destroy(gameObject);
        Destroy(hpText.gameObject);
        Destroy(dataText.gameObject);
        Destroy(ammoText.gameObject);
        GameObject d = Instantiate(deathObject, new Vector3(0, 0, 0), Quaternion.identity);
        d.transform.GetChild(1).gameObject.GetComponent<Text>().text = "YOU DIED";
        int profit = data * moneyPerData;
        d.transform.GetChild(2).gameObject.GetComponent<Text>().text = "Data Stolen: " + data + "GB, Profit: $" + (profit-gearCost) +
            ", Died on Wave: " + GameObject.Find("Game Manager").GetComponent<GameManager>().wave;
        GameObject.Find("Game Manager").GetComponent<GameManager>().money += profit;
        SaveManager.SaveGame(new Save(GameObject.Find("Game Manager").GetComponent<GameManager>().money), SaveTracker.instance.id);
    }

    public void Fire()
    {
        print("Burst Firing");

        Weapon w = weapon[heldWeapon];

        if ((w.consumeBurstAmmo && w.ammo > 0) || !w.consumeBurstAmmo)
        {
            GameObject bullet = Instantiate(weapon[heldWeapon].bullet,
                weapon[heldWeapon].bulletSpawn.position, weapon[heldWeapon].bulletSpawn.rotation);

            bullet.transform.Rotate(Random.Range(-w.spread, w.spread), Random.Range(-w.spread, w.spread), Random.Range(-w.spread, w.spread));

            if (w.bulletType == 0)
            {
                print("Firing Bullet");
                Bullet bScript = bullet.GetComponent<Bullet>();
                bScript.Fire(weapon[heldWeapon].bulletSpeed, weapon[heldWeapon].damage);
            }
            else if (w.bulletType == 1)
            {
                print("Firing Rocket");
                print(bullet.name);
                Rocket rScript = bullet.GetComponent<Rocket>();
                rScript.Launch(weapon[heldWeapon].bulletSpeed, weapon[heldWeapon].damage,
                    weapon[heldWeapon].explosionSize, weapon[heldWeapon].explosionLifetime);
            }

            if (w.spawnFX)
            {
                GameObject shootFX = Instantiate(shootEffect,
                    weapon[heldWeapon].bulletSpawn.position, weapon[heldWeapon].bulletSpawn.rotation);
            }

            gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            if (weapon[heldWeapon].playBurstSound)
                gameObject.GetComponent<AudioSource>().PlayOneShot(weapon[heldWeapon].shotSound);

            if (w.consumeBurstAmmo) w.ammo--;

            recoil.addRecoil();

            print("Burst Fired");
        }
    }

    public void ApplyUpgrades()
    {
        int[] levels = SaveManager.LoadGame(SaveTracker.instance.id).upgrades;
        Upgrade[] upgrades = UpgradeList.upgrades;

        for(int x = 0; x < levels.Length; x++)
        {
            upgrades[x].level = levels[x];
        }

        //Reinforced Bullets
        if (levels[0] > 0)
        {
            weapon[0].damage *= upgrades[0].effect * levels[0];
            weapon[1].damage *= upgrades[0].effect * levels[0];
        }

        //Extended Magazines
        if (levels[1] > 0)
        {
            float a = weapon[0].ammoPerMag;
            a *= (upgrades[1].effect * levels[1]) + 1;
            weapon[0].ammoPerMag = (int)a;
            a = weapon[1].ammoPerMag;
            a *= (upgrades[1].effect * levels[1]) + 1;

            weapon[1].ammoPerMag = (int)a;
            weapon[0].ammo = weapon[0].ammoPerMag;
            weapon[0].unloadedAmmo = weapon[0].ammoPerMag * 2;
            weapon[1].ammo = weapon[1].ammoPerMag;
            weapon[1].unloadedAmmo = weapon[1].ammoPerMag * 2;
        }

        //Hardy Biotech
        if(levels[2] > 0)
        {
            maxHP += (int)upgrades[2].effect * levels[2];
            hp = maxHP;
        }

        //Quickened Chambers
        if(levels[3] > 0)
        {
            weapon[0].speed -= (weapon[0].speed * upgrades[3].effect * levels[3]);
            weapon[1].speed -= (weapon[1].speed * upgrades[3].effect * levels[3]);
        }

        //Barrel Stabilization
        if(levels[4] > 0)
        {
            weapon[0].spread -= (weapon[0].spread * upgrades[4].effect * levels[4]);
            weapon[1].spread -= (weapon[1].spread * upgrades[4].effect * levels[4]);
        }

        //Resistant Biotech
        if (levels[5] > 0) armor.damageReduction += upgrades[5].effect * levels[5];

        //Leg Augments
        if(levels[6] > 0)
        {
            GetComponent<FirstPersonController>().m_RunSpeed += upgrades[6].effect * levels[6];
            GetComponent<FirstPersonController>().m_WalkSpeed += upgrades[6].effect * levels[6];
        }

        //Power Boots
        if(levels[7] > 0) GetComponent<FirstPersonController>().m_RunSpeed += upgrades[7].effect * levels[7];

        //Boot Thrusters
        if (levels[8] > 0) GetComponent<FirstPersonController>().m_JumpSpeed += upgrades[8].effect * levels[8];

        //Barrel Suppression
        if(levels[9] > 0)
        {
            weapon[0].spawnFX = false;
            weapon[1].spawnFX = false;
        }

        //Market Manipulation
        if (levels[10] > 0) moneyPerData += (int) upgrades[10].effect * levels[10];

        //Fast Autohack
        if(levels[11] > 0)
        {
            autoHackSpeed -= upgrades[11].effect * levels[11];
            InvokeRepeating("StealData", autoHackSpeed, autoHackSpeed);
        }

        //Fast Reloading
        if(levels[12] > 0)
        {
            weapon[0].reloadSpeed -= (weapon[0].reloadSpeed * upgrades[12].effect * levels[12]);
            weapon[1].reloadSpeed -= (weapon[1].reloadSpeed * upgrades[12].effect * levels[12]);
        }
    }
}
