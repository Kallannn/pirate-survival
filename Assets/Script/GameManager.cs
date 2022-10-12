using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] MatchConfiguration matchConfig;
    [SerializeField] private int score;
    [Space]
    [SerializeField] private GameObject player;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private PrefabPool blackCannonBallPool;
    [SerializeField] private PrefabPool whiteCannonBallPool;
    [SerializeField] private EnemySpawner shooterSpawner;
    [SerializeField] private EnemySpawner suicideSpawner;
    [SerializeField] private MainGameUIManager mainGameUIManager;

    private int matchEndTime = 0;
    private bool matchEnded = false;

    private void Start()
    {
        matchEndTime = (int)Time.timeSinceLevelLoad + matchConfig.matchDuration;

        score = 0;

        shooterSpawner.SetIntervalAndStart(matchConfig.shooterSpawnTime, this);
        suicideSpawner.SetIntervalAndStart(matchConfig.suicideSpawnTime, this);

        player.GetComponent<Player>().SetGameManagerReference(this);

        InvokeRepeating("UpdateUITime",0,1);
    }

    private void UpdateUITime()
    {
        int timeLeft = matchEndTime - (int)Time.timeSinceLevelLoad;
        if (timeLeft <= 0 && !matchEnded)
        {
            mainGameUIManager.SetTimeText(timeLeft);
            EndMatch("Time's up");
        }
        else if(!matchEnded)
        {
            mainGameUIManager.SetTimeText(timeLeft);
        }
    }

    private void UpdateUIScore()
    {
        mainGameUIManager.SetScore(score);
    }

    public void AddScore(int score)
    {
        this.score += score;
        UpdateUIScore();
    }

    public void ConfigureEnemy(EnemyShip enemy)
    {
        enemy.SetGameManager(this);
        enemy.SetPlayerReference(player.transform);
        enemy.SetCannonBallPool(blackCannonBallPool, blackCannonBallPool);
        enemy.SetPathFinder(pathfinder);
    }

    public void EndMatch(string gameOverText = "Game Over")
    {
        if (!matchEnded)
        {
            matchEnded = true;
            
            player.GetComponent<Player>().DisableInputs();

            shooterSpawner.gameObject.SetActive(false);
            suicideSpawner.gameObject.SetActive(false);

            mainGameUIManager.ShowGameOverScreen(score, gameOverText);
        }
    }

    public void PlayAgainButtonClick()
    {
        string thisSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(thisSceneName);
    }

    public void MainMenuButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
