using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JobMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectJob(int i)
    {
        Log.LogMsg("Starting CreateJ...");
        StartCoroutine(CreateJ());

    }

    IEnumerator CreateJ()
    {
        CreateJInfo(1);
        yield return new WaitForSeconds(0.01f);

        Log.LogMsg("Starting StartJob...");
        StartCoroutine(StartJob());
    }

    public static void CreateJInfo(int floor)
    {
        Log.LogMsg("Creating j...");
        GameObject j = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
        j.name = Random.Range(0, FactionList.factions.Count) + " " + Random.Range(0, ThemeList.themes.Count) + " " + 5 + " " + floor;
        j.tag = "Job Info";
        DontDestroyOnLoad(j);
        Log.LogMsg("Created j");
    }

    public static IEnumerator StartJob()
    {
        Log.LogMsg("Started StartJob");
        yield return new WaitForSeconds(0.01f);
        Log.LogMsg("Loading scene...");
        SceneManager.LoadScene("Job");
    }
}
