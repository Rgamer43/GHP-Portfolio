using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Weapon : Item
{

    public AudioClip sfx;
    public AudioClip reloadSfx;

    public string category;

    public bool unique;

    public int fireMode;
    public float damage;
    public int rpm;
    public float spread;
    public int magSize;
    public float reloadSpeed;
    public bool heavy;
    public float[] range;
    public float velocity;
    public float fov;
    public float recoil;
    public int bulletCount;

    public int ammo;

    public GameObject bullet;

    public Inventory mods;

    public Sprite sprite;

    public Weapon() {}

    public Weapon(string n, string d, string cat, bool uniq, int lv, string sprt, string sfx, string rsfx, int fm, float dmg, int rpm, float sprd, int mag, float reload, bool hvy, float[] rang, float vel, float fov, float rcl, int bltCnt) : base(n, d, lv)
    {
        category = cat;
        unique = uniq;

        fireMode = fm;
        damage = dmg;
        this.rpm = rpm;
        spread = sprd;
        magSize = mag;
        reloadSpeed = reload;
        heavy = hvy;
        range = rang;
        velocity = vel;
        this.fov = fov;
        recoil = rcl;
        bulletCount = bltCnt;

        ammo = magSize;

        this.sfx = Resources.Load<AudioClip>("SFX/" + sfx);
        this.reloadSfx = Resources.Load<AudioClip>("SFX/" + rsfx);
        bullet = Resources.Load<GameObject>("Prefabs/Bullet");

        mods = new Inventory(new Modification().GetType(), 3);

        sprite = Resources.Load<Sprite>("Sprites/Weapons/" + sprt);
    }

    public Weapon(int i) : base(i)
    {
        Weapon b = (Weapon) ItemList.items[i];

        category = b.category;
        unique = b.unique;

        fireMode = b.fireMode;
        damage = b.damage;
        rpm = b.rpm;
        spread = b.spread;
        magSize = b.magSize;
        reloadSpeed = b.reloadSpeed;
        heavy = b.heavy;
        range = b.range;
        velocity = b.velocity;
        fov = b.fov;
        recoil = b.recoil;
        bulletCount = b.bulletCount;

        ammo = magSize;

        mods = new Inventory(new Modification().GetType(), 3);
        
        bullet = b.bullet;
        sfx = b.sfx;
        reloadSfx = b.reloadSfx;
        sprite = b.sprite;

        sprite = b.sprite;
    }

    public Weapon(string n, string d, string cat, bool uniq, int lv, string sprt, string sfx, string rsfx, int fm, float dmg, int rpm, float sprd, int mag, float reload, bool hvy, float[] rang, float vel, float fov, float rcl, int bltCnt, string bulletType) : this(n, d, cat, uniq, lv, sprt, sfx, rsfx, fm, dmg, rpm, sprd, mag, reload, hvy, rang, vel, fov, rcl, bltCnt)
    {
        Log.LogMsg(bulletType);
        bullet = Resources.Load<GameObject>("Prefabs/" + bulletType);
        Log.LogMsg(bullet.name);
    }

    public void ApplyMods()
    {
        for (int i = 0; i < mods.Count(); i++)
        {
            Modification m = mods.Get(i).GetMod();

            damage *= 1 + m.damage;
            rpm = (int) (rpm * (1 + m.rpm));
            spread *= 1 + m.spread;
            magSize = (int) Mathf.Round(magSize * (1 + m.magSize));
            reloadSpeed *= 1 + m.reloadSpeed;
            range[0] *= 1 + m.range[0];
            range[1] *= 1 + m.range[1];
            fov *= 1 + m.fov;
            recoil *= 1 + m.recoil;
        }
    }

    public void RemoveMods()
    {
        for (int i = 0; i < mods.Count(); i++)
        {
            Modification m = mods.Get(i).GetMod();

            damage /= 1 + m.damage;
            rpm = (int)(rpm / (1 + m.rpm));
            spread /= 1 + m.spread;
            magSize = (int)Mathf.Round(magSize / (1 + m.magSize));
            reloadSpeed /= 1 + m.reloadSpeed;
            range[0] /= 1 + m.range[0];
            range[1] /= 1 + m.range[1];
            fov *= 1 + m.fov;
            recoil *= 1 + m.recoil;
        }
    }

    public bool AddMod(Modification mod)
    {
        if(mods.HasSpace())
        {
            mods.AddItem(mod, 1);
        }

        return false;
    }

    public void SetLevel(int l)
    {
        int diff = level - l;
        if (diff > 0)
            damage *= Mathf.Pow(Options.DAMAGE_SCALING, diff);
        else if (diff < 0)
            damage /= Mathf.Pow(Options.DAMAGE_SCALING, -diff);

        level = l;
    }
}
