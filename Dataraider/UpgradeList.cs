using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeList : MonoBehaviour
{
    public static Upgrade[] upgrades =
    {
        new Upgrade("Reinforced Bullets", "+5% damage", 0.05f, 100, 1.1f, 40),
        new Upgrade("Extended Magazines", "+25% ammo per magazine", 0.25f, 500, 2.5f, 8),
        new Upgrade("Hardy Biotech", "+50 health", 50.0f, 1000, 2f, 4),
        new Upgrade("Quickened Chambers", "-10% firerate", 0.2f, 250, 1.3f, 9),
        new Upgrade("Barrel Stabilization", "-20% bullet spread", 0.2f, 200, 2f, 4),
        new Upgrade("Resistant Biotech", "+5% damage reduction", 5.00f, 5000, 1.5f, 3),
        new Upgrade("Leg Augments", "+0.5 walk and run speed", 0.5f, 650, 1.5f, 6),
        new Upgrade("Power Boots", "+1 run speed", 1f, 600, 2f, 5),
        new Upgrade("Boot Thrusters", "+1 jump speed", 1f, 500, 1.5f, 6),
        new Upgrade("Barrel Suppression", "Hide shooting flash", 1f, 2500, 1f, 1),
        new Upgrade("Market Manipulation", "+$1 per GB of data", 1f, 2500, 1.2f, 20),
        new Upgrade("Fast Autohack", "-1 second to get free data", 1f, 3000, 1.3f, 9),
        new Upgrade("Fast Reloading", "-10% reload speed", 0.1f, 1000, 1.2f, 9)
    };
}
