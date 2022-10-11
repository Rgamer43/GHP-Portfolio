using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenu : MonoBehaviour
{

    public GameObject canvas;

    public GameObject playButton;
    public GameObject playMenu;
    public GameObject playMenuInstance;

    public GameObject newGame;
    public GameObject loadGame;

    public Transform startPos;

    public GameObject credits;
    public GameObject modeSelect;
    public GameObject shop;

    protected string[] levels = { 
        "Bridge",
        "Facility",
        "Wintery",
        "Docks",
        "Office"
    };

    // Start is called before the first frame update
    void Start()
    {
        startPos = playButton.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayClicked()
    {
        startPos = playButton.transform;
        //playButton.SetActive(false);
        newGame = Instantiate(playMenu, startPos.position, startPos.rotation);
        playMenuInstance = newGame;

        newGame.transform.SetParent(canvas.transform);

        //StartCoroutine(ButtonMover());
    }

    public void OnCreditsClicked()
    {
        Instantiate(credits, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void OnExitClicked()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    //IEnumerator ButtonMover()
    //{
    //    //Edit what x has to less than in the for loop to change duration, edit what x is divided by to change speed
    //    for(int x = 0; x < 50; x++)
    //    {
    //        Vector3 loadPos = new Vector3(startPos.position.x, startPos.position.y + (x / 2), startPos.position.z);
    //        Vector3 newPos = new Vector3(startPos.position.x, startPos.position.y - (x / 2), startPos.position.z);
    //
    //        loadGame.transform.position = loadPos;
    //        newGame.transform.position = newPos;
    //
    //        yield return new WaitForSecondsRealtime(0.0000001f);
    //    }
    //}

    public void Load(int id)
    {
        SaveManager.LoadGame(id);
        OpenModeSelect();
    }

    public void New(int id)
    {
        Save save = new Save(0, new int[Save.upgradesNum]);
        SaveManager.SaveGame(save, id);
        SaveManager.LoadGame(id);
        OpenModeSelect();
    }

    public void StartGame()
    {
        int i = Random.Range(0, levels.Length);
        print(i);
        SceneManager.LoadScene(levels[i]);
    }

    public void OpenModeSelect()
    {
        Instantiate(modeSelect);
        Destroy(playMenuInstance);
    }

    public void OpenStoryMenu()
    {

    }

    public void OpenShop()
    {
        Instantiate(shop);
    }
}
