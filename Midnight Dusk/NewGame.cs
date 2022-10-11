using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGame : MonoBehaviour
{

    public InputField inputField;
    public GameObject error;

    public void Back()
    {
        Destroy(gameObject);
    }

    public void Enter()
    {
        if(inputField.text != "")
        {
            print("Checking if save exists...");

            if (File.Exists(Application.persistentDataPath + "/" + inputField.text + ".save")) error.SetActive(true);
            else
            {
                SaveManager.CreateSave(inputField.text);
                Destroy(gameObject);
                SaveManager.currentSave = inputField.text;
                SceneManager.LoadScene("Hub");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Back();
        else if (Input.GetKeyDown(KeyCode.Return)) Enter();
    }
}
