using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class DataPersistence : MonoBehaviour
{
    public static DataPersistence instance;
    public int ChosenNumOfRowsData;
    public int ChosenNumOfColsData;
    public int bestScore;

    private void Awake()
    {
        
        if(instance==null)
        {
            instance=this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void DataStore()
    {
        ChosenNumOfRowsData = FirstStart.ChosenNumOfRows;
        ChosenNumOfColsData = FirstStart.ChosenNumOfCols;
    }

    [System.Serializable]
    class SaveInfo
    {
       public int bestScoreSaved;
    }

    public void SaveData()
    {
        SaveInfo data = new SaveInfo();
        data.bestScoreSaved =bestScore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath+"/savefile.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath +"/savefile.json";
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveInfo data = JsonUtility.FromJson<SaveInfo>(json);

            bestScore = data.bestScoreSaved;
        }
    }
}
