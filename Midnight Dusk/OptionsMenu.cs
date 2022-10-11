using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;

public class OptionsMenu : MonoBehaviour
{

    public GameObject displayMenu;
    public GameObject audioMenu;
    public GameObject performanceMenu;
    public GameObject achievementsMenu;
    public GameObject creditsMenu;

    public void DisplayMenu()
    {
        Instantiate(displayMenu, new Vector3(0, 0, 0), Quaternion.identity);
    }
    public void AudioMenu()
    {
        Instantiate(audioMenu, new Vector3(0, 0, 0), Quaternion.identity);
    }
    public void PerformanceMenu()
    {
        Instantiate(performanceMenu, new Vector3(0, 0, 0), Quaternion.identity);
    }
    public void AchievementsMenu()
    {
        Instantiate(achievementsMenu, new Vector3(0, 0, 0), Quaternion.identity);
    }
    public void CreditsMenu()
    {
        Instantiate(creditsMenu, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void Back()
    {
        Destroy(gameObject);
    }
}
