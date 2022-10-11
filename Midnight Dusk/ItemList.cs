using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemList
{
    public static Item[] items =
    {
        new Weapon("Test AR", "An assault rifle for testing", "Assault Rifle", false, -1, "Makeshift AR", "TestGunshot", "TestReload", 0, 3, 600, 10, 40, 2f, false, new float[]{4, 12}, 5f, 7, 3f, 1),
        new Weapon("Test Pistol", "A pistol for testing", "Pistol", false, -1, "SMG", "TestGunshot", "TestReload", 1, 10, 250, 4, 10, 2f, false, new float[]{5, 12}, 5f, 6, 2f, 1),
        new Armor("Hoodie", "A dark hoodie", -1, 0, 0, 0, 0, 0, 100, 20, 100),
        new Armor("Conspicuous Hoodie", "A REALLY dark hoodie", -1, 0, 0, 0, 0, 0, 100, 20, 100),
        new Weapon("The RazeR", "Raze's best selling rifle the RazeR is all around well made.", "Combat Rifle", false, 6, "RF Assault rifle MK l", "TestGunshot", "TestReload", 1, 6, 300, 4, 30, 1.2f, false, new float[]{30, 65}, 5f, 8, 2.5f, 1),
        new Weapon("Executioner", "A powerful black market sniper.", "Sniper Rifle", true, 6, "The Executioner", "TestGunshot", "TestReload", 1, 130, 60, 0, 1, 4, true, new float[]{55, 90}, 6.5f, 11, 5, 1),
        new Weapon("RF Shotgun MK II", "A midrange shotgun. Commonly used by the Red Finger's enforcers for intimidation.", "Shotgun", false, 5, "RSG12", "TestGunshot", "TestReload", 1, 6.5f, 95, 15, 8, 2, true, new float[]{10, 30}, 5f, 7, 6.5f, 4),
        new Weapon("No Witnesses", "Probably created by a criminal. Moderate firerate and medium damage make this a jack of all trades weapon.", "Battle Rifle", false, 5, "Wyvern", "TestGunshot", "TestReload", 0, 6, 420, 2, 35, 3.5f, false, new float[]{20, 35}, 4.5f, 7.5f, 5.5f, 1),
        new Weapon("Selenium Scorcher", "A failed flamethrower design. Originally intended to be a high quality military weapon, it's design was very flawed and it barely works.", "Flamethrower", false, 3, "HERTZ-3", "TestGunshot", "TestReload", 0, 25, 60, 35, 8, 2.25f, false, new float[]{10, 15}, 0.5f, 6.5f, 0, 1, "Flame"),
        new Weapon("Final Punch", "A huge, unwieldy shotgun. Short range, but absolutely destroys anything dumb enough to get close.", "Shotgun", true, 9, "Shotgun", "TestGunshot", "TestReload", 1, 5, 50, 5, 1, 2, true, new float[]{3, 6}, 5f, 6, 8, 20),
    };
}
