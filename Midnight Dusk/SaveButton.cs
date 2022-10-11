using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveButton : MonoBehaviour
{

    public GameObject playMenu;

    public void Play()
    {
        SaveManager.currentSave = gameObject.name;
        SceneManager.LoadScene("Hub");
    }

    public void Delete()
    {
        SaveManager.DeleteSave(gameObject.name);
        Destroy(GameObject.Find("Play Menu(Clone)"));
        Instantiate(playMenu, new Vector3(0, 0, 0), Quaternion.identity);
    }
}