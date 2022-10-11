using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public GameObject playMenu;
    public GameObject optionsMenu;

    public void Play()
    {
        Instantiate(playMenu, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void Options()
    {
        Instantiate(optionsMenu, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnApplicationQuit()
    {
        Log.streamWriter.Close();
    }
}
