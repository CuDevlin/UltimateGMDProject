using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject enemyPrefab;
    public int maxEnemies = 20;
    public float spawnInterval = 2f;
    public float spawnDistance = 20f;

    private Transform player;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private float spawnTimer;

    private bool spawningEnabled = false; // New flag to control spawning

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!spawningEnabled || player == null)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    public void SetSpawningEnabled(bool enable)
    {
        spawningEnabled = enable;
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPosition = GetSpawnPosition();
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);

        EnemyController enemyComponent = newEnemy.GetComponent<EnemyController>();
        if (enemyComponent != null)
        {
            enemyComponent.OnEnemyDestroyed += HandleEnemyDestroyed;
        }
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 playerPos = player.position;
        Vector2 playerDirection = player.GetComponent<Rigidbody2D>()?.linearVelocity.normalized ?? Vector2.zero;
        float playerVelocity = player.GetComponent<Rigidbody2D>()?.linearVelocity.magnitude ?? 0f;

        if (playerVelocity > 0.5f && Random.value > 0.5f)
        {
            float angleOffset = Random.Range(-10f, 10f);
            Vector2 spawnDirection = Quaternion.Euler(0, 0, angleOffset) * playerDirection;
            return playerPos + spawnDirection.normalized * spawnDistance;
        }

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return playerPos + randomDirection * spawnDistance;
    }

    private void HandleEnemyDestroyed(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }
}