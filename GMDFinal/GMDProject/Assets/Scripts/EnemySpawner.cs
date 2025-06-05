using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject enemyTypeA;
    public GameObject enemyTypeB;
    public GameObject enemyTypeC;

    public int baseLimitA = 5;
    public int baseLimitB = 5;
    public int baseLimitC = 5;

    [Tooltip("How much to reduce spawn interval every time limits increase")]
    public float spawnIntervalDecreaseStep = 0.2f;

    [Tooltip("Minimum allowed spawn interval")]
    public float minimumSpawnInterval = 0.2f;

    public float spawnInterval = 2f;
    public float spawnDistance = 20f;

    [Header("Limit Scaling")]
    public float limitIncreaseInterval = 30f;
    public int limitIncreaseAmount = 1;

    private int currentLimitA;
    private int currentLimitB;
    private int currentLimitC;

    private float spawnTimer = 0f;
    private float limitTimer = 0f;

    private Transform player;
    private bool spawningEnabled = false;

    private List<GameObject> activeEnemiesA = new();
    private List<GameObject> activeEnemiesB = new();
    private List<GameObject> activeEnemiesC = new();

    private void Start()
    {
        currentLimitA = baseLimitA;
        currentLimitB = baseLimitB;
        currentLimitC = baseLimitC;
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
            else
                return;
        }

        if (!spawningEnabled)
            return;

        spawnTimer += Time.deltaTime;
        limitTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            TrySpawnEnemy();
            spawnTimer = 0f;
        }

        if (limitTimer >= limitIncreaseInterval)
        {
            currentLimitA += limitIncreaseAmount;
            currentLimitB += limitIncreaseAmount;
            currentLimitC += limitIncreaseAmount;

            spawnInterval = Mathf.Max(spawnInterval - spawnIntervalDecreaseStep, minimumSpawnInterval);

            limitTimer = 0f;

            Debug.Log($"[SPAWNER] Limits ↑ A={currentLimitA}, B={currentLimitB}, C={currentLimitC}, New Interval: {spawnInterval:0.00}s");
        }
    }

    public void SetSpawningEnabled(bool enable)
    {
        spawningEnabled = enable;
    }

    private void TrySpawnEnemy()
    {
        // Randomly choose type, but only if under limit
        List<int> validTypes = new();

        if (activeEnemiesA.Count < currentLimitA) validTypes.Add(0);
        if (activeEnemiesB.Count < currentLimitB) validTypes.Add(1);
        if (activeEnemiesC.Count < currentLimitC) validTypes.Add(2);

        if (validTypes.Count == 0)
            return; // All types at limit

        int chosen = validTypes[Random.Range(0, validTypes.Count)];
        Vector2 pos = GetSpawnPosition();

        GameObject prefab = null;
        List<GameObject> list = null;

        switch (chosen)
        {
            case 0: prefab = enemyTypeA; list = activeEnemiesA; break;
            case 1: prefab = enemyTypeB; list = activeEnemiesB; break;
            case 2: prefab = enemyTypeC; list = activeEnemiesC; break;
        }

        GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
        list.Add(enemy);

        EnemyController ec = enemy.GetComponent<EnemyController>();
        if (ec != null)
            ec.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 playerPos = player.position;
        Vector2 dir = Random.insideUnitCircle.normalized;

        if (player.TryGetComponent(out Rigidbody2D rb))
        {
            Vector2 vel = rb.linearVelocity;
            if (vel.magnitude > 0.5f && Random.value > 0.5f)
            {
                float offset = Random.Range(-10f, 10f);
                dir = Quaternion.Euler(0, 0, offset) * vel.normalized;
            }
        }

        return playerPos + dir * spawnDistance;
    }

    private void HandleEnemyDestroyed(GameObject enemy)
    {
        activeEnemiesA.Remove(enemy);
        activeEnemiesB.Remove(enemy);
        activeEnemiesC.Remove(enemy);
    }

    public void ClearEnemies()
    {
        foreach (var enemy in activeEnemiesA) if (enemy) Destroy(enemy);
        foreach (var enemy in activeEnemiesB) if (enemy) Destroy(enemy);
        foreach (var enemy in activeEnemiesC) if (enemy) Destroy(enemy);

        activeEnemiesA.Clear();
        activeEnemiesB.Clear();
        activeEnemiesC.Clear();
    }
}
