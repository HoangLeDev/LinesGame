using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private Text score;
    [SerializeField]
    private GameObject gameOverMenuUI;
    private int bestScore;
    private int currentMaxScore;
    public Text bestScoreDisplay;

    private void Awake()
    {
        DataPersistence.instance.LoadData();
        bestScoreDisplay.text = ("Best Score: " + DataPersistence.instance.bestScore);
    }

    public void UpdateScore()
    {
        score.text = "Score: "+ Ball.score;
    }

    public void GameOverMenu()
    {
        gameOverMenuUI.SetActive(true);
        Ball.isGameOver=true;
        if(DataPersistence.instance != null)
        {
            currentMaxScore= Ball.score;
            if(currentMaxScore>DataPersistence.instance.bestScore)
            {
                DataPersistence.instance.bestScore=currentMaxScore;
                DataPersistence.instance.SaveData();
                bestScoreDisplay.text = ("Best Score: " + DataPersistence.instance.bestScore);
            }
        }
    }
}
