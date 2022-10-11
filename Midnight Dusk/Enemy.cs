using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public GameObject player;
    public GameObject gun;
    public GameObject gunHolder;
    public SpriteRenderer gunSprite;
    public Transform bulletSpawn;
    public SpriteRenderer sprite;

    public bool facesLeftByDefault = false;

    public Room room;

    [SerializeField]
    public Weapon weapon;

    public List<Vector2> path;
    public float speed;

    public float maxHP;
    public float hp;
    public float defense;

    public bool hasLOS = false;

    public int canFire = 0;
    public float reloading = -1;
    public Transform[] recoilPos;
    public int recoiling = -1;

    public float timeSinceReachedPoint = 0f;
    public bool shouldPathfind = false;
    public bool pathfinding = false;

    public bool flies;

    public Vector2 position;

    public GameObject damageMarkerFull;
    public GameObject damageMarkerPartial;
    public GameObject damageMarkerNone;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DrawPath", 2f);
        sprite = GetComponent<SpriteRenderer>();
        damageMarkerFull = Resources.Load<GameObject>("Prefabs/Damage Marker - Full");
        damageMarkerPartial = Resources.Load<GameObject>("Prefabs/Damage Marker - Partial");
        damageMarkerNone = Resources.Load<GameObject>("Prefabs/Damage Marker - None");
    }

    public void Init()
    {
        hp = maxHP;
        CheckLOS();
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        if (player == null) player = GameObject.Find("Player");

        Vector3 aimAngleDifference = player.transform.position - transform.position;
        aimAngleDifference.Normalize();
        float aimAngleZ = Mathf.Atan2(aimAngleDifference.y, aimAngleDifference.x) * Mathf.Rad2Deg;
        gunHolder.transform.rotation = Quaternion.Euler(0, 0, aimAngleZ);
        float rotation_z = Mathf.Atan2(aimAngleDifference.y, aimAngleDifference.x) * Mathf.Rad2Deg;
        gun.transform.rotation = Quaternion.Euler(0f, 0f, rotation_z);
        if (rotation_z > 90 || rotation_z < -90)
        {
            gunSprite.flipY = true;
        }
        else
        {
            gunSprite.flipY = false;
        }

        if (path.Count > 0)
        {
            OnMove();
            timeSinceReachedPoint += Time.deltaTime;
            if (timeSinceReachedPoint > 5) shouldPathfind = true;
            transform.position = Vector2.MoveTowards(transform.position, path[0], speed * Time.deltaTime);

            if ((path[0].x > 0 && facesLeftByDefault) || (path[0].x < 0 && !facesLeftByDefault)) sprite.flipX = true;
            else sprite.flipX = false;

            if (transform.position.x == path[0].x && transform.position.y == path[0].y || Vector2.Distance(transform.position, path[0]) < 0.05)
            {
                timeSinceReachedPoint = 0f;
                shouldPathfind = false;
                path.RemoveAt(0);
                Log.LogMsg("Enemy reached pathfind node.");
            }
        }

        if(hasLOS && canFire == 0)
        {
            if (weapon.ammo > 0)
            {
                if (reloading > 0) reloading = -1;

                canFire = weapon.fireMode;
                if (canFire == 0) canFire = 1;
                Fire();
            }
            else StartReload();
        }
        if (weapon.ammo == 0) StartReload();

        try
        {

            if (recoiling != -1) gun.transform.position = Vector2.MoveTowards(gun.transform.position, recoilPos[recoiling].position, 0.005f * weapon.recoil);

            if (recoiling == 1 && gun.transform.position == recoilPos[recoiling].position)
            {
                recoiling = 0;
                if (canFire == 0) gun.transform.rotation = recoilPos[0].rotation;
            }
            else if (recoiling == 0 && gun.transform.position == recoilPos[recoiling].position)
            {
                recoiling = -1;
                if (canFire == 0) gun.transform.rotation = recoilPos[0].rotation;
            }
        }
        catch (System.NullReferenceException e)
        {

        }
    }

    public void CheckLOS()
    {
        if (room.isPlayerInRoom)
        {
            RaycastHit2D hit = Physics2D.Raycast(bulletSpawn.position,
                new Vector2(player.transform.position.x - bulletSpawn.position.x, player.transform.position.y - bulletSpawn.position.y));
            if (hit.transform.gameObject.name == "Player" && Vector2.Distance(bulletSpawn.position, player.transform.position) < weapon.range[1]) hasLOS = true;
            else hasLOS = false;
        }

        if (hp > 0) Invoke("CheckLOS", Options.OCCUPIED_ROOM_TICK_RATE);
    }
    public void Fire()
    {
        if (canFire > 0 && weapon.ammo > 0 && Vector2.Distance(player.transform.position, bulletSpawn.position) < 1.1 * weapon.range[1])
        {
            OnFire();
            for (int i = 0; i < weapon.bulletCount; i++)
            {
                GameObject bullet = Instantiate(weapon.bullet, bulletSpawn.position, bulletSpawn.rotation);
                bullet.transform.Rotate(new Vector3(0, 0, Random.Range(-weapon.spread / 2, weapon.spread / 2)));
                bullet.GetComponent<Bullet>().Fire(weapon.velocity, weapon.damage, weapon.range, false);

                Debug.DrawRay(bulletSpawn.position, bullet.transform.rotation.eulerAngles, Color.yellow, 1);
            }

            recoiling = 1;

            weapon.ammo--;

            //print("RPM=" + weapon.rpm);
            //print(60.0f / weapon.rpm);
            Invoke("ReduceCanFire", 60.0f / weapon.rpm);
            Invoke("Fire", 60.0f / weapon.rpm);
        }
    }

    private void ReduceCanFire()
    {
        canFire--;
        if (weapon.ammo == 0) StartReload();
    }
    public void StartReload()
    {
        reloading = weapon.reloadSpeed;
        Invoke("Reload", .1f);
    }

    public void Reload()
    {
        //print(reloading);
        if (reloading > 0)
        {
            reloading -= 0.1f;
            Invoke("Reload", .1f);
        }
        else if (reloading <= .5f && reloading != -1)
        {
            reloading = -1;
            weapon.ammo = weapon.magSize;
            shouldPathfind = true;
            //print("Finished reload");
        }
    }

    public float TakeDamage(float dmg)
    {
        if (room.isPlayerInRoom)
        {
            dmg *= 1 - defense;

            if (dmg > 0) Destroy(Instantiate(Resources.Load<GameObject>("Prefabs/BloodEffect"), transform.position, Quaternion.identity), 12f);

            hp -= dmg;

            if (hp <= 0)
            {
                // Create death particle effect
                // (We don't actually have a death particle effect yet)
                // Delete the enemy
                Destroy(gameObject);
                room.enemies.Remove(this);

                if(room.enemies.Count <= 0)
                    Destroy(Instantiate(Resources.Load<GameObject>("Prefabs/Room Complete")), 2f);
            }
        }

        return dmg;
    }

    public void DrawPath()
    {
        if(path.Count > 0)
        {
            Debug.DrawLine(transform.position, path[0], Color.green, Options.OCCUPIED_ROOM_TICK_RATE);
            for(int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.green, Options.OCCUPIED_ROOM_TICK_RATE);
            }
        }

        if (hp > 0) Invoke("DrawPath", Options.OCCUPIED_ROOM_TICK_RATE);
    }

    public float TakeDamage(float dmg, float baseDmg, Vector2 pos)
    {
        RenderDamageMarker(baseDmg, dmg, pos);
        return TakeDamage(dmg);
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

    public void OnMove()
    {

    }

    public void OnFire()
    {

    }
}
