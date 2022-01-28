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

    public void UpdateScore()
    {
        score.text = "Score: "+ Ball.score;
    }

    public void GameOverMenu()
    {
        gameOverMenuUI.SetActive(true);
        Ball.isGameOver=true;
    }
}
