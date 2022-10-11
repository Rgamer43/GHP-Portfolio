using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public static Player instance;

    public Rigidbody2D rb;
    public Camera camera;

    public Armor armor;
    [SerializeField]
    public Inventory cybernetics;
    [SerializeField]
    public Inventory weapons;
    [SerializeField]
    public Inventory inventory;

    public int heldWeapon = 0;

    public readonly float BASE_SPEED = 25;
    public float speed;

    public float maxHP;
    public float hp;
    public float maxShield;
    public float shield;
    public float shieldRegen;
    public float defense;

    public float shieldRegenTimer = 0;

    public int canFire = 0;
    public float reloading = -1;
    public Transform bulletSpawn;
    public GameObject gunHolder;
    public GameObject gun;
    public SpriteRenderer gunSprite;
    public GameObject reloadMeter;
    public float reloadMeterInitialWidth;

    public double dashForce;
    public float dashCooldown = 2f;
    public float dashTime = 0;
    public GameObject dashCooldownMeter;
    public float dashCooldownMeterInitialWidth;

    public GameObject fragGrenade;
    private bool fragKeyPressed = false;

    public readonly double BASE_DASH_FORCE = 900;
    public readonly float SHIELD_COOLDOWN_AFTER_DMG = 5f;

    public readonly int BASE_INVENTORY_SIZE = 20;

    public Text ammoText;
    public Text hpText;
    public Text shieldText;

    public Transform[] recoilPos;
    public int recoiling = -1;

    public bool controlsEnabled = true;

    public bool inventoryOpen = false;
    public GameObject inventoryMenuPrefab;
    public GameObject inventoryMenu;

    public float targetFOV = 7;

    public Vector2 position;

    public float maxDamageVignetteStrength;
    public float damageVignetteDuration; // The length for which a damage vignette will linger without being damaged again
    public float healingVignetteStrength;
    public float healingVignetteDuration; // The length for whichi a healing vignette will linger without healing again
    public GameObject vignetteOverlay;
    public Q_Vignette_Single vignetteOverlayScript;
    public float vignetteRecedeStrength; // The variable subtracted from the vignette strength
    public float vignetteRecedeSpeed; // The time between frames when the vignette will get smaller
    public float vignetteRecedeTimer; // A timer (updated every frame) that controls the receding of the vignette

    public GameObject damageMarkerFull;
    public GameObject damageMarkerPartial;
    public GameObject damageMarkerNone;

    public GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) instance = this;
        else
        {
            Debug.LogError("Attempting to create two players!!!");
            Destroy(this);

            //Set the process priority
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
        }

        transform.name = "Player";

        rb = GetComponent<Rigidbody2D>();
        camera = transform.GetChild(0).GetComponent<Camera>();

        if (SaveManager.IsNewGame()) NewGame();
        else
        {
            SaveManager.Load();
        }

        // Get the damage vignette script to be manipulated later
        vignetteOverlay = GameObject.Find("Vignette Overlay");
        vignetteOverlayScript = vignetteOverlay.GetComponent<Q_Vignette_Single>();

        dashCooldownMeterInitialWidth = dashCooldownMeter.GetComponent<RectTransform>().rect.width;
        reloadMeterInitialWidth = reloadMeter.GetComponent<RectTransform>().rect.width;

        damageMarkerFull = Resources.Load<GameObject>("Prefabs/Damage Marker - Full");
        damageMarkerPartial = Resources.Load<GameObject>("Prefabs/Damage Marker - Partial");
        damageMarkerNone = Resources.Load<GameObject>("Prefabs/Damage Marker - None");
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;

        if (controlsEnabled)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();
            float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            gun.transform.rotation = Quaternion.Euler(0f, 0f, rotation_z);
            if(rotation_z > 90 || rotation_z < -90){
                gunSprite.flipY = true;
            } else {
                gunSprite.flipY = false;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            camera.gameObject.transform.position = new Vector3(
                (transform.position.x + transform.position.x + transform.position.x + transform.position.x + transform.position.x + transform.position.x + mousePos.x) / 7, 
                (transform.position.y + transform.position.y + transform.position.y + transform.position.y + transform.position.y + transform.position.y + mousePos.y) / 7, 
                -10);

            if (canFire < 0) canFire = 0;

            //0 should be left click
            if (((Input.GetMouseButton(Keybindings.fire) && GetHeldWeapon().fireMode == 0) || (Input.GetMouseButtonDown(Keybindings.fire) && GetHeldWeapon().fireMode > 0)) && canFire == 0 && reloading == -1)
            {
                if (GetHeldWeapon().ammo > 0)
                {
                    if (reloading > 0) reloading = -1;

                    canFire = weapons.Get(heldWeapon).GetWeapon().fireMode;
                    if (canFire == 0) canFire = 1;
                    Fire();
                }
                else StartReload();
            }
            else if (Input.GetKeyDown(Keybindings.reload) && GetHeldWeapon().ammo < GetHeldWeapon().magSize) StartReload();

            if (Input.GetKeyDown(Keybindings.equipment1) && weapons.Count() > 0)
            {
                SetHeldWeapon(0);
            }
            else if (Input.GetKeyDown(Keybindings.equipment2) && weapons.Count() > 1)
            {
                SetHeldWeapon(1);
            }

            if (dashTime > 0) dashTime -= Time.deltaTime;
            if (dashTime < 0) dashTime = 0;

            if(Input.GetKeyDown(Keybindings.dash) && dashTime == 0)
            {
                Vector2 dForce = new Vector2((float)(difference.x * dashForce), (float)(difference.y * dashForce));
                float fMod = Vector2.Distance(new Vector2(0, 0), difference);
                Log.LogMsg("Dash Force (Unmodified): " + dashForce);
                dForce.x /= fMod; dForce.y /= fMod;
                rb.AddForce(dForce);
                Log.LogMsg("Dash Force: " + dashForce);
                Log.LogMsg("Difference: " + difference.x + ", " + difference.y);
                Log.LogMsg("FMod: " + fMod);
                Log.LogMsg("DForce: " + dForce);
                Log.LogMsg("Total Force: " + Vector2.Distance(new Vector2(0, 0), dForce));
                Log.LogMsg("Dashing with force: " + dForce);
                dashTime = dashCooldown;
            }
            dashCooldownMeter.GetComponent<RectTransform>().sizeDelta = new Vector2(dashCooldownMeterInitialWidth - (dashCooldownMeterInitialWidth * (dashTime / dashCooldown)), dashCooldownMeter.GetComponent<RectTransform>().rect.height);
        }

        try
        {

            if (recoiling != -1)
            {
               if(recoiling == 1)  gunHolder.transform.position = Vector2.MoveTowards(gunHolder.transform.position, recoilPos[recoiling].position, 0.05f * GetRecoilMod() * GetHeldWeapon().recoil);
               else gunHolder.transform.position = Vector2.MoveTowards(gunHolder.transform.position, recoilPos[recoiling].position, 0.045f * GetRecoilMod() / GetHeldWeapon().recoil);
            }

            if (recoiling == 1 && gunHolder.transform.position == recoilPos[recoiling].position)
            {
                recoiling = 0;
                if (canFire == 0) gunHolder.transform.rotation = recoilPos[0].rotation;
            }
            else if (recoiling == 0 && gunHolder.transform.position == recoilPos[recoiling].position)
            {
                recoiling = -1;
                if (canFire == 0) gunHolder.transform.rotation = recoilPos[0].rotation;
            }

            UpdateUI();
        } catch(System.NullReferenceException e)
        {

        }

        int recoilAmt = 25;

        //if (recoiling == 1) gunHolder.transform.Rotate(0, 0, Time.deltaTime * GetRecoilMod() * GetHeldWeapon().recoil * recoilAmt * -1);
        //else if (recoiling == 0 && gunHolder.transform.rotation.z < recoilPos[0].rotation.z) gunHolder.transform.Rotate(0, 0, Time.deltaTime * GetRecoilMod() * GetHeldWeapon().recoil * recoilAmt);

        if (shield < maxShield)
        {
            shieldRegenTimer += Time.deltaTime;
            if (shieldRegenTimer >= shieldRegen / maxShield)
            {
                // Vignette to show healing
                vignetteOverlayScript.mainScale = healingVignetteStrength;
                vignetteOverlayScript.mainColor = new Color(0, 255, 255); // Set the vignette color to teal
                vignetteRecedeTimer = healingVignetteDuration;

                shield++;
                shieldRegenTimer = 0;
            }
        }

        if(vignetteRecedeTimer >= 0){
            vignetteRecedeTimer -= Time.deltaTime;
        } else {
            vignetteOverlayScript.mainScale -= vignetteRecedeStrength;
            vignetteRecedeTimer = vignetteRecedeSpeed;
        }

        if (Input.GetKeyDown(Keybindings.inventory) && inventoryOpen == false)
        {
            inventoryOpen = true;
            controlsEnabled = false;

            Instantiate(inventoryMenuPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        }
        else if ((Input.GetKeyDown(Keybindings.inventory) || Input.GetKeyDown(KeyCode.Escape)) && inventoryOpen == true)
        {
            inventoryOpen = false;
            controlsEnabled = true;

            inventoryMenu = GameObject.Find("Inventory(Clone)");
            Destroy(inventoryMenu);
        }

        if (camera.orthographicSize + 1.5f > targetFOV) camera.orthographicSize -= Time.deltaTime * (camera.orthographicSize - targetFOV) * 5;
        else if (camera.orthographicSize - 1.5f < targetFOV) camera.orthographicSize -= Time.deltaTime * (camera.orthographicSize - targetFOV) * 5;
        else camera.orthographicSize = targetFOV;

        if (camera.orthographicSize > 20f || camera.orthographicSize < 2f) camera.orthographicSize = targetFOV;
    }

    public void FixedUpdate()
    {
        if(controlsEnabled)
        {
            Vector2 motion = new Vector2();

            if (Input.GetKey(Keybindings.moveUp) && !Input.GetKey(Keybindings.moveDown)) motion.y = speed;
            else if (Input.GetKey(Keybindings.moveDown) && !Input.GetKey(Keybindings.moveUp)) motion.y = -speed;

            if (Input.GetKey(Keybindings.moveLeft) && !Input.GetKey(Keybindings.moveRight)) motion.x = -speed;
            else if (Input.GetKey(Keybindings.moveRight) && !Input.GetKey(Keybindings.moveLeft)) motion.x = speed;

            if (motion.x != 0 && motion.y != 0)
            {
                motion.x *= 0.7f;
                motion.y *= 0.7f;
            }

            rb.AddForce(motion);

            // Grrnades!
            if(Input.GetKey(Keybindings.fragGrenade)){
                if(!fragKeyPressed){
                    Instantiate(fragGrenade, bulletSpawn.position, bulletSpawn.rotation);
                    fragKeyPressed = true;
                }
            } else {
                fragKeyPressed = false;
            }
        }
    }

    public void NewGame()
    {
        Log.LogImportant("Setting up new game...");
        weapons = new Inventory(new Weapon().GetType(), 2);
        weapons.AddItem(new Weapon(ItemIDs.TEST_AR), 1);
        weapons.AddItem(new Weapon(ItemIDs.TEST_PISTOL), 1);
        armor = new Armor(ItemIDs.HOODIE);
        inventory = new Inventory(new Item().GetType(), BASE_INVENTORY_SIZE);
        cybernetics = new Inventory(new Cybernetic().GetType(), 3);
        gunSprite.sprite = GetHeldWeapon().sprite;

        inventory.AddItem(new Armor(ItemIDs.HOODIE), 1);
        inventory.AddItem(new Armor(ItemIDs.HOODIE), 1);
        inventory.AddItem(new Weapon(ItemIDs.TEST_AR), 1);
        inventory.AddItem(new Armor(ItemIDs.CONSPICUOUS_HOODIE), 1);
        inventory.AddItem(new Weapon(ItemIDs.THE_RAZER), 1);
        inventory.AddItem(new Weapon(ItemIDs.EXECUTIONER), 1);
        inventory.AddItem(new Weapon(ItemIDs.RF_SHOTGUN_MK_II), 1);
        inventory.AddItem(new Weapon(ItemIDs.NO_WITNESSES), 1);
        inventory.AddItem(new Weapon(ItemIDs.SELENIUM_SCORCHER), 1);
        inventory.AddItem(new Weapon(ItemIDs.FINAL_PUNCH), 1);

        CalculateStats();
        hp = maxHP;
        shield = maxShield;

        SaveManager.Save();

        Log.LogImportant("Finished new game set up");
    }

    public void CalculateStats()
    {
        CalculateSpeed();
        CalculateFOV();
        CalculateHealth();
        CalculateShield();
        CalculateShieldRegen();
        CalculateDefense();

        dashForce = BASE_DASH_FORCE;
    }

    public float CalculateSpeed()
    {
        float s;

        if (armor != null) s = BASE_SPEED * (1 + armor.moveSpeed);
        else s = BASE_SPEED;

        for (int i = 0; i < weapons.Count(); i++) if (weapons.Get(i) != null) if(weapons.Get(i).GetWeapon().heavy) s *= (1f-Options.HEAVY_MOVE_PENALTY);
        for (int i = 0; i < cybernetics.Count(); i++) 
            if (cybernetics.Get(i) != null)
                s *= cybernetics.Get(i).GetCybernetic().moveSpeed;

        speed = s;
        return s;
    }

    public float CalculateFOV()
    {
        float f = weapons.Get(heldWeapon).GetWeapon().fov;

        if (cybernetics == null) cybernetics = new Inventory(new Cybernetic().GetType(), 3);
        for (int i = 0; i < cybernetics.Count(); i++) if (cybernetics.Get(i) != null) f *= cybernetics.Get(i).GetCybernetic().fov;

        targetFOV = f;
        return f;
    }

    public float CalculateHealth()
    {
        float h;

        if (armor != null) h = armor.hp;
        else h = 10;

        for (int i = 0; i < cybernetics.Count(); i++) if (cybernetics.Get(i) != null) h *= cybernetics.Get(i).GetCybernetic().hp;

        maxHP = h;
        return h;
    }

    public float CalculateShield()
    {
        float s;

        if (armor != null) s = armor.shield;
        else s = 25;


        for (int i = 0; i < cybernetics.Count(); i++) if (cybernetics.Get(i) != null) s *= cybernetics.Get(i).GetCybernetic().shield;

        maxShield = s;
        return s;
    }

    public float CalculateShieldRegen()
    {
        float s;

        if (armor != null) s = armor.shieldRegen;
        else s = 5;


        for (int i = 0; i < cybernetics.Count(); i++) if (cybernetics.Get(i) != null) s *= cybernetics.Get(i).GetCybernetic().shieldRegen;

        shieldRegen = s;
        return s;
    }

    public float CalculateDefense()
    {
        float d;

        if (armor != null) d = armor.defense;
        else d = 0;

        for (int i = 0; i < cybernetics.Count(); i++) if (cybernetics.Get(i) != null) d *= cybernetics.Get(i).GetCybernetic().defense;

        if (d > .98f) d = .98f;

        defense = d;
        return d;
    }

    public float GetDamageMod()
    {
        float m = 1;

        for (int i = 0; i < cybernetics.Count(); i++) m += cybernetics.Get(i).GetCybernetic().damage;

        return m;
    }

    public float GetRecoilMod()
    {
        float r = 1;

        for (int i = 0; i < cybernetics.Count(); i++) r += cybernetics.Get(i).GetCybernetic().recoil;

        return r;
    }

    public float GetReloadMod()
    {
        float r = 1;

        for (int i = 0; i < cybernetics.Count(); i++) r += cybernetics.Get(i).GetCybernetic().reloadSpeed;

        return r;
    }

    public float GetSpreadMod()
    {
        float s = 1;

        for (int i = 0; i < cybernetics.Count(); i++) s += cybernetics.Get(i).GetCybernetic().spread;

        return s;
    }

    public float[] GetRangeMod()
    {
        float[] r = new float[2];

        for (int i = 0; i < cybernetics.Count(); i++)
        {
            r[0] += cybernetics.Get(i).GetCybernetic().range[0];
            r[1] += cybernetics.Get(i).GetCybernetic().range[1];
        }

        return r;
    }

    public void Fire()
    {
        if (canFire > 0 && GetHeldWeapon().ammo > 0)
        {
            for (int i = 0; i < GetHeldWeapon().bulletCount; i++)
            {
                GameObject bullet = Instantiate(GetHeldWeapon().bullet, bulletSpawn.position, bulletSpawn.rotation);
                bullet.transform.Rotate(new Vector3(0, 0, Random.Range(-GetHeldWeapon().spread / 2, GetHeldWeapon().spread / 2)));
                bullet.GetComponent<Bullet>().Fire(GetHeldWeapon().velocity, GetHeldWeapon().damage * GetDamageMod(), GetHeldWeapon().range, true);
                Log.LogMsg("Firing player bullet");
            }
            recoiling = 1;

            GetHeldWeapon().ammo--;
            

            //Log.LogMsg("RPM=" + GetHeldWeapon().rpm);
            //Log.LogMsg(60.0f / GetHeldWeapon().rpm);
            Invoke("ReduceCanFire", 60.0f / GetHeldWeapon().rpm);
            Invoke("Fire", 60.0f / GetHeldWeapon().rpm);
        }
        float ammo = (float)GetHeldWeapon().ammo;
        float magSize = (float)GetHeldWeapon().magSize;
        //Debug.Log(ammo);
        //Debug.Log(magSize);
        reloadMeter.GetComponent<RectTransform>().sizeDelta = new Vector2(reloadMeterInitialWidth * (ammo / magSize), reloadMeter.GetComponent<RectTransform>().rect.height);
    }

    private void ReduceCanFire()
    {
        canFire--;
        if (GetHeldWeapon().ammo == 0) StartReload();
    }

    public Weapon GetHeldWeapon()
    {
        try { return weapons.Get(heldWeapon).GetWeapon(); }
        catch(System.NullReferenceException e) { return null; };
    }

    public void SetHeldWeapon(int i)
    {
        canFire = 0;
        reloading = -1;
        heldWeapon = i;
        gunSprite.sprite = GetHeldWeapon().sprite;
        CalculateFOV();
        Log.LogMsg("Bar %: " + (GetHeldWeapon().ammo / GetHeldWeapon().magSize));
        Log.LogMsg("Ammo: " + GetHeldWeapon().ammo + ", MagSize: " + GetHeldWeapon().magSize);
        reloadMeter.GetComponent<RectTransform>().sizeDelta = new Vector2(reloadMeterInitialWidth * ((float)GetHeldWeapon().ammo / (float)GetHeldWeapon().magSize), reloadMeter.GetComponent<RectTransform>().rect.height);
    }

    public void UpdateUI()
    {
        hpText.text = Mathf.Round((hp/maxHP) * 100) + "%";
        shieldText.text = Mathf.Floor((shield/maxShield) * 100) + "%";

        if (reloading == -1) ammoText.text = GetHeldWeapon().ammo + "/" + GetHeldWeapon().magSize;
        else ammoText.text = "--/" + GetHeldWeapon().magSize;
    }

    public void StartReload()
    {
        reloading = GetHeldWeapon().reloadSpeed * GetReloadMod();
        Invoke("Reload", .1f);
    }

    public void Reload()
    {
        //Log.LogMsg(reloading);
        if (reloading > 0)
        {
            reloading -= 0.1f;
            Invoke("Reload", .1f);
        }
        else if(reloading <= .5f && reloading != -1)
        {
            reloading = -1;
            GetHeldWeapon().ammo = GetHeldWeapon().magSize;
            Log.LogMsg("Finished reload");
        }
        float maxReloadTime = GetHeldWeapon().reloadSpeed * GetReloadMod();
        reloadMeter.GetComponent<RectTransform>().sizeDelta = new Vector2(reloadMeterInitialWidth - (reloadMeterInitialWidth * ((reloading / maxReloadTime) < 0 ? 0 : (reloading / maxReloadTime))), reloadMeter.GetComponent<RectTransform>().rect.height);
    }

    public float TakeDamage(float dmg, float baseDmg, Vector2 pos)
    {
        RenderDamageMarker(baseDmg, dmg, pos);
        return TakeDamage(dmg);
    }
    
    public float TakeDamage(float dmg)
    {
        shieldRegenTimer = -SHIELD_COOLDOWN_AFTER_DMG;

        dmg *= 1 - defense;

        shield -= dmg;

        if (dmg > 0) Destroy(Instantiate(Resources.Load<GameObject>("Prefabs/BloodEffect"), transform.position, Quaternion.identity), 12f);

        float vignetteStrength = (maxHP + maxShield) / (hp + shield) > maxDamageVignetteStrength ? maxDamageVignetteStrength : (maxHP + maxShield) / (hp + shield);
        vignetteOverlayScript.mainScale = vignetteStrength;
        vignetteOverlayScript.mainColor = new Color(255, 0, 0); // Set the vignette color to red
        vignetteRecedeTimer = damageVignetteDuration;

        if(shield < 0)
        {
            hp -= Mathf.Abs(shield);
            shield = 0;
        }

        if (hp <= 0)
        {
            GameOver();
        }

        return dmg;
    }

    public float TakeDamage(float dmg, Vector2 pos)
    {
        float d = TakeDamage(dmg);
        RenderDamageMarker(dmg, d, pos);
        return d;
    }

    public void RenderDamageMarker(float baseDmg, float d, Vector2 pos)
    {
        GameObject marker;
        if (d == 0)
        {
            GameObject m = Instantiate(damageMarkerNone, new Vector3(pos.x, pos.y, 2), Quaternion.identity);
            m.GetComponent<TextMeshPro>().text = "0";
            marker = m;
        }
        else if (d < baseDmg)
        {
            GameObject m = Instantiate(damageMarkerPartial, new Vector3(pos.x, pos.y, 2), Quaternion.identity);
            m.GetComponent<TextMeshPro>().text = (Mathf.Round(d * 10) / 10).ToString();
            marker = m;
        }
        else
        {
            GameObject m = Instantiate(damageMarkerFull, new Vector3(pos.x, pos.y, 2), Quaternion.identity);
            m.GetComponent<TextMeshPro>().text = (Mathf.Round(d * 10) / 10).ToString();
            marker = m;
        }

        marker.transform.SetParent(GameObject.Find("UI").transform);
        Destroy(marker, Options.DMG_MARKER_LIFETIME);
    }

    public void GameOver()
    {
        Destroy(gameObject);

        Instantiate(gameOverScreen);
        Instantiate(new GameObject()).AddComponent<Camera>();

        armor.AddLives(-1);
        if (armor.lives == 0) armor = new Armor(ItemIDs.HOODIE);

        for(int i = 0; i < weapons.Count(); i++)
        {
            weapons.Get(i).item.AddLives(-1);
            if (weapons.Get(i).item.lives == 0)
            {
                weapons.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < cybernetics.Count(); i++)
        {
            cybernetics.Get(i).item.AddLives(-1);
            if (cybernetics.Get(i).item.lives == 0)
            {
                cybernetics.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < inventory.Count(); i++)
        {
            inventory.Get(i).item.AddLives(-1);
            if (inventory.Get(i).item.lives == 0)
            {
                inventory.RemoveAt(i);
                i--;
            }
        }

        SaveManager.Save();
    }

    public void OnApplicationQuit()
    {
        Log.streamWriter.Close();
    }
}