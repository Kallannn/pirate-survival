using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainGameUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [Space]
    [TextArea]
    [SerializeField] private string scoreStartText = "Score: ";
    [TextArea]
    [SerializeField] private string timeStartText = "Time Left: ";


    private string SecondsToTimeFormat(float time)
    {
        int fullMinutes = (int)time / 60;
        int seconds = (int)time - (fullMinutes * 60);

        return fullMinutes + ":" + ((seconds < 10) ? "0" + seconds : seconds);
    }

    public void SetTimeText(int time)
    {
        timeText.text = timeStartText + SecondsToTimeFormat(time);
    }

    public void SetScore(int score)
    {
        scoreText.text = timeStartText + score;
    }

    public void ShowGameOverScreen(int score, string gameOverText = "Game Over")
    {
        timeText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);

        this.gameOverText.text = gameOverText;
        this.finalScoreText.text = scoreStartText + score;

        gameOverScreen.SetActive(true);
    }
}
