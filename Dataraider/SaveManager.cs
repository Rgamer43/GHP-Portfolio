using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{

    //public static int id = 0;
    
    public static void SaveGame(Save save, int id)
    {
        print(save.upgrades[0]);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save" + id + ".save");
        bf.Serialize(file, save);
        file.Close();
    }

    public static Save LoadGame(int id)
    {
        if(File.Exists(Application.persistentDataPath + "/save" + id + ".save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save" + id + ".save", FileMode.Open);
            Save save = (Save) bf.Deserialize(file);
            file.Close();

            //SaveManager.id = id;

            GameObject tracker = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
            tracker.AddComponent<SaveTracker>();
            DontDestroyOnLoad(tracker);
            SaveTracker.instance.id = id;

            return new Save(save.money, save.upgrades);
        }

        print("ERROR: No save file found at " + Application.persistentDataPath + "/save" + id + "s.save");
        return null;
    }

}
