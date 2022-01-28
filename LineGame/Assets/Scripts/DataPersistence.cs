using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataPersistence : MonoBehaviour
{
    public static DataPersistence instance;
    public int ChosenNumOfRowsData;
    public int ChosenNumOfColsData;

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
}
