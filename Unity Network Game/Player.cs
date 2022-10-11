using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{

    public PlayerHandler handler;
    public Rigidbody2D rb;

    public NetworkVariable<float> speed = new NetworkVariable<float>(), jumpForce = new NetworkVariable<float>();
    public NetworkVariable<int> jumps = new NetworkVariable<int>(), maxJumps = new NetworkVariable<int>(), health = new NetworkVariable<int>(), canFire = new NetworkVariable<int>();
    public NetworkVariable<PlayerClass> playerClass = new NetworkVariable<PlayerClass>();
    public NetworkVariable<Weapon> weapon = new NetworkVariable<Weapon>();
    public NetworkVariable<bool> reloading = new NetworkVariable<bool>();

    public GameObject weaponHolder, HUD;
    public Transform bulletSpawn, spawn;
    public Text ammo, healthUI, gameStatus, score;
    public SpriteRenderer sprite;
    public LineRenderer line;

    public bool localFire = true;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();

        if (IsServer || IsHost)
        {
            speed.Value = 3;
            maxJumps.Value = 1;
            jumps.Value = maxJumps.Value;
            jumpForce.Value = 425;
            //health.Value = 3;
            reloading.Value = false;
            canFire.Value = -1;
        }

        print("Spawned player " + OwnerClientId);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (gameObject.activeInHierarchy)
            {
                if (!HUD.activeInHierarchy) HUD.SetActive(true);

                int bScore = GameManager.score[0],
                    rScore = GameManager.score[1];

                if (bScore < 200 && rScore < 200)
                {

                    if (!reloading.Value) ammo.text = "Ammo: " + weapon.Value.ammo + "/" + weapon.Value.magSize;
                    else ammo.text = "Ammo: --/" + weapon.Value.magSize;

                    healthUI.text = "Health: " + health.Value;

                    gameStatus.text = "";
                    if (handler.team.Value == 0) gameStatus.text += "<color=blue>You are <b>blue</b></color>";
                    else gameStatus.text += "<color=red>You are <b>red</b></color>";

                    if (GameManager.instance.hillControl.Value == -1) gameStatus.text += "\nHill is neutral";
                    else if (GameManager.instance.hillControl.Value == -2) gameStatus.text += "\nHill is contested";
                    else if (GameManager.instance.hillControl.Value == 0) gameStatus.text += "\n<color=blue>Hill is blue</color>";
                    else if (GameManager.instance.hillControl.Value == 1) gameStatus.text += "\n<color=red>Hill is red</color>";

                    if (GameManager.instance.oddControl.Value == -1) gameStatus.text += "\nOddball is neutral";
                    else if (GameManager.instance.oddControl.Value == 0) gameStatus.text += "\n<color=blue>Oddball is blue</color>";
                    else if (GameManager.instance.oddControl.Value == 1) gameStatus.text += "\n<color=red>Oddball is red</color>";
                    if (bScore == rScore) score.text = "<color=blue>" + bScore + "</color> - <color=red>" + rScore + "</color>";
                    else if (bScore > rScore) score.text = "<color=blue>" + bScore + " - </color><color=red>" + rScore + "</color>";
                    else if (bScore < rScore) score.text = "<color=blue>" + bScore + "</color><color=red> - " + rScore + "</color>";
                } else if (bScore >= 200)
                {
                    healthUI.text = "<color=blue>Blue Wins!</color>";
                    ammo.text = "<color=blue>Blue Wins!</color>";
                    gameStatus.text = "<color=blue>Blue Wins!</color>";

                    DieServerRPC(4);
                } else if (rScore >= 200)
                {
                    healthUI.text = "<color=red>Red Wins!</color>";
                    ammo.text = "<color=red>Red Wins!</color>";
                    gameStatus.text = "<color=red>Red Wins!</color>";

                    DieServerRPC(4);
                }

                //print("Current Score: " + bScore + " - " + rScore);

                if (handler.team.Value == 0) sprite.color = Color.blue;
                else sprite.color = Color.red;
                SetColorServerRPC(sprite.color);

                float input = 0;
                if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) input = -speed.Value;
                else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) input = speed.Value;
                RegisterInputServerRPC(input);

                if (jumps.Value > 0 && Input.GetKeyDown(KeyCode.Space)) JumpServerRPC();

                //rotation
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 0;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                Vector3 objectPos = weaponHolder.transform.position;
                mousePos.x = mousePos.x - objectPos.x;
                mousePos.y = mousePos.y - objectPos.y;
                float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
                PointGunServerRPC(angle);

                if (localFire && canFire.Value == -1 && !reloading.Value && ((weapon.Value.fireMode == 0 && Input.GetMouseButton(0)) || Input.GetMouseButtonDown(0))) {
                    if (weapon.Value.ammo > 0) FireServerRPC(true);
                    else ReloadServerRPC();

                    localFire = false;
                    Invoke("ResetLocalFire", (float)(60f / (float)weapon.Value.rpm));
                }

                if (Input.GetKeyDown(KeyCode.R)) ReloadServerRPC();
            }
        }

        if(IsServer)
        {
            if (canFire.Value < -1) canFire.Value = -1;

            if (handler.team.Value == 0) spawn = GameObject.Find("Blue Spawn").transform;
            else spawn = GameObject.Find("Red Spawn").transform;
        }
    }

    [ServerRpc]
    public void DieServerRPC(float delay)
    {
        Invoke("Die", delay);
    }

    public void Die()
    {
        TakeDamage(100);
    }

    [ServerRpc]
    public void SetColorServerRPC(Color color)
    {
        SetColorClientRPC(color);
    }

    [ClientRpc]
    public void SetColorClientRPC(Color color)
    {
        sprite.color = color;
    }

    public void ResetLocalFire()
    {
        localFire = true;
    }

    [ServerRpc]
    public void FireServerRPC(bool initial)
    {
        if(initial) canFire.Value = weapon.Value.fireMode;

        print("Firing initially: " + initial);

        if (canFire.Value > -1)
        {
            weapon.Value = ChangeAmmo(weapon.Value, -1);

            print("Raycasting...");
            Vector2 dir = bulletSpawn.transform.position - weaponHolder.transform.position;
            dir.x *= UnityEngine.Random.Range(1 - (weapon.Value.spread / 2), 1 + (weapon.Value.spread / 2));
            dir.x+= UnityEngine.Random.Range(-weapon.Value.spread/2, weapon.Value.spread/2);
            dir.y *= UnityEngine.Random.Range(1 - (weapon.Value.spread), 1 + (weapon.Value.spread));
            dir.y += UnityEngine.Random.Range(-weapon.Value.spread / 2, weapon.Value.spread / 2);
            RaycastHit2D hit = Physics2D.Raycast(bulletSpawn.transform.position, dir, 300);
            if (hit.rigidbody != null)
            {
                print("Shot from " + OwnerClientId + " hit " + hit.rigidbody.gameObject.name);
                hit.rigidbody.AddForce(dir * 6);
                if (hit.rigidbody.gameObject.tag == "Player") hit.rigidbody.GetComponent<Player>().TakeDamage(weapon.Value.dmg);
                line.SetPositions(new Vector3[] { bulletSpawn.transform.position, hit.transform.position });
                SetLineClientRPC(new Vector3[] { bulletSpawn.transform.position, hit.transform.position });
            }
            else
            {
                print("Nothing hit");
                line.SetPositions(new Vector3[] { bulletSpawn.transform.position, dir * 100 });
                SetLineClientRPC(new Vector3[] { bulletSpawn.transform.position, dir * 100 });
            }
            Debug.DrawRay(bulletSpawn.transform.position, dir * 100, Color.yellow, 0.75f);

            Invoke("ClearLine", 0.5f);

            canFire.Value--;
            print("Time before fire again: " + (float)(60f / (float)weapon.Value.rpm));
            print("CanFire: " + canFire.Value);
            if(weapon.Value.ammo > 0 && canFire.Value >= 0) Invoke("ContinueFire", (float)(60f / (float)weapon.Value.rpm));
        }
    }

    [ClientRpc]
    public void SetLineClientRPC(Vector3[] p)
    {
        line.SetPositions(p);
    }

    public void ClearLine()
    {
        line.SetPositions(new Vector3[] {bulletSpawn.position, bulletSpawn.position});
        SetLineClientRPC(new Vector3[] { bulletSpawn.position, bulletSpawn.position });
    }

    public void ContinueFire()
    {
        print("Continuing fire...");
        if(canFire.Value > -1) canFire.Value--;
        if (canFire.Value == 0) canFire.Value = -1;
        if (canFire.Value > 0)
        {
            print("Firing again...");
            weapon.Value = ChangeAmmo(weapon.Value, -1);

            print("Raycasting...");
            RaycastHit2D hit = Physics2D.Raycast(bulletSpawn.transform.position, bulletSpawn.transform.position - weaponHolder.transform.position, 300);
            if (hit.rigidbody != null)
            {
                print("Shot from " + OwnerClientId + " hit " + hit.rigidbody.gameObject.name);
                if (hit.rigidbody.gameObject.tag == "Player") hit.rigidbody.GetComponent<Player>().TakeDamage(weapon.Value.dmg);
            }
            else print("Nothing hit");
            Debug.DrawRay(bulletSpawn.transform.position, (bulletSpawn.transform.position - weaponHolder.transform.position) * 100, Color.yellow, 0.75f);

            canFire.Value--;
            print("CanFire: " + canFire.Value);
            if (weapon.Value.ammo > 0 && canFire.Value > 0) Invoke("ContinueFire", (float)(60f / (float)weapon.Value.rpm));
        }
    }

    [ServerRpc]
    public void ReloadServerRPC()
    {
        reloading.Value = true;
        Invoke("FinishReload", weapon.Value.reloadSpeed);
    }

    public void TakeDamage(int amt)
    {
        print("Took " + amt + " damage, previous health: " + health.Value);
        health.Value -= amt;
        print("New health: " + health.Value);

        if (health.Value <= 0)
        {
            print("Player " + OwnerClientId + " died");

            if (GameManager.instance == null) Debug.LogWarning("GameManager.instance is null!");

            if (handler.team.Value == 0)
            {
                GameManager.instance.AddPoints(1, 1);
            }
            else
            {
                GameManager.instance.AddPoints(0, 1);
            }

            handler.DeathClientRPC();
            transform.position = spawn.position;
        }
    }

    public void FinishReload()
    {
        reloading.Value = false;
        canFire.Value = -1;
        weapon.Value = SetAmmo(weapon.Value, weapon.Value.magSize);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            jumps.Value = maxJumps.Value;

            if (collision.gameObject.name == "Kill Zone") TakeDamage(100);
        }
    }

    [ServerRpc]
    public void RegisterInputServerRPC(float input)
    {
        rb.AddForce(new Vector2(input, 0));
    }

    [ServerRpc]
    public void JumpServerRPC()
    {
        jumps.Value--;
        rb.AddForce(new Vector2(0, jumpForce.Value));
        print("Player " + OwnerClientId + " jumped");
    }

    [ServerRpc]
    public void SetClassServerRPC(PlayerClass c)
    {
        playerClass.Value = c;
        speed.Value = 4.5f;

        switch (c)
        {
            case PlayerClass.Soldier:
                weapon.Value = new Weapon
                {
                    dmg = 1, rpm = 300, magSize = 10, reloadSpeed = 4, fireMode = 0, spread = 0.1f
                };
                break;


            case PlayerClass.Sniper:
                weapon.Value = new Weapon
                {
                    dmg = 6,
                    rpm = 120,
                    magSize = 1,
                    reloadSpeed = 6,
                    fireMode = 1,
                    spread = 0
                };
                health.Value = 4;
                speed.Value *= 0.85f;
                break;


            case PlayerClass.Raider:
                weapon.Value = new Weapon
                {
                    dmg = 1,
                    rpm = 300,
                    magSize = 10,
                    reloadSpeed = 4,
                    fireMode = 1,
                    spread = 0.5f
                };
                health.Value = 4;
                speed.Value *= 1.1f;
                break;


            case PlayerClass.Defender:
                weapon.Value = new Weapon
                {
                    dmg = 1,
                    rpm = 180,
                    magSize = 6,
                    reloadSpeed = 4.5f,
                    fireMode = 1,
                    spread = 0.05f
                };
                health.Value = 12;
                speed.Value *= 0.8f;
                break;
        }

        weapon.Value = SetAmmo(weapon.Value, weapon.Value.magSize);
        if (health.Value <= 0) health.Value = 6;
    }

    public Weapon ChangeAmmo(Weapon w, int amt)
    {
        return new Weapon
        {
            dmg = w.dmg,
            rpm = w.rpm,
            magSize = w.magSize,
            reloadSpeed = w.reloadSpeed,
            fireMode = w.fireMode,
            spread = w.spread,
            ammo = w.ammo + amt
        };
    }

    public Weapon SetAmmo(Weapon w, int amt)
    {
        return new Weapon
        {
            dmg = w.dmg,
            rpm = w.rpm,
            magSize = w.magSize,
            reloadSpeed = w.reloadSpeed,
            fireMode = w.fireMode,
            spread = w.spread,
            ammo = amt
        };
    }

    public void Enable()
    {
        EnableServerRPC();
    }

    [ServerRpc]
    public void EnableServerRPC()
    {
        gameObject.SetActive(true);
        EnableClientRPC();
    }

    [ClientRpc]
    public void EnableClientRPC()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        DisableServerRPC();
    }

    [ServerRpc]
    public void DisableServerRPC()
    {
        gameObject.SetActive(false);
        DisableClientRPC();
    }

    [ClientRpc]
    public void DisableClientRPC()
    {
        gameObject.SetActive(false);
    }

    [ServerRpc]
    public void PointGunServerRPC(float angle)
    {
        weaponHolder.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //print("Pointing gun towards " + angle + ", weapon rot: " + weaponHolder.transform.rotation.z);
    }

}
