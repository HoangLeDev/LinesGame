using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FirstStart : MonoBehaviour
{
    public GameObject levelPanel;
    public static int ChosenNumOfRows;
    public static int ChosenNumOfCols;
    public void StartGame()
    {
        levelPanel.SetActive(true);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
    public void Level7x7()
    {
        ChosenNumOfRows = 7;
        ChosenNumOfCols = 7;
        DataPersistence.instance.DataStore();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    public void Level9x9()
    {
        ChosenNumOfRows = 9;
        ChosenNumOfCols = 9;
        DataPersistence.instance.DataStore();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
