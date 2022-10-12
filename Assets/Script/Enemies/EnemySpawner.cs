using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector3[] spawnPoints;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameManager gameManager;

    public void SetIntervalAndStart(float interval, GameManager manager)
    {
        gameManager = manager;
        InvokeRepeating("SpawnEnemy", interval, interval);
    }

    private void SpawnEnemy()
    {
        var enemy = Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity);

        Debug.Log("Spawning " + enemy.name);

        gameManager.ConfigureEnemy(enemy.GetComponent<EnemyShip>());
        enemy.SetActive(true);
    }

    private Vector3 GetRandomPosition()
    {
        return spawnPoints[Random.Range(0,spawnPoints.Length - 1)];
    }
}
