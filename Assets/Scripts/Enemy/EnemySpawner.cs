﻿using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public int maxNumberOfEnemies = 4;
    public float distanceToPlayerToStartSpawning = 5f;
    public float spawnDelay = 1f;

    private bool canStartSpawning = false;
    private float spawnTimer = 0f;
    private int numberOfEnemiesSpawned = 0;
    private Transform spawnPoint;

    void Start()
    {
        // Tìm GameObject có tag "SpawnerEnemy" và gán giá trị cho biến spawnPoint
        GameObject spawnPointObject = GameObject.FindGameObjectWithTag("SpawnerEnemy");
        if (spawnPointObject != null)
        {
            spawnPoint = spawnPointObject.transform;
        }
        else
        {
            Debug.LogError("No object found with tag 'SpawnerEnemy'");
        }

        InvokeRepeating("SpawnEnemy", 0f, spawnRate);
    }

    void Update()
    {
        // Kiểm tra nếu điều kiện khoảng cách giữa người chơi và quái vật được thỏa mãn
        if (!canStartSpawning)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, FindObjectOfType<PlayerController>().transform.position);
            if (distanceToPlayer <= distanceToPlayerToStartSpawning)
            {
                canStartSpawning = true;
            }
        }

        // Đếm thời gian giữa các lần spawn
        spawnTimer += Time.deltaTime;
    }

    void SpawnEnemy()
    {
        if (canStartSpawning && CheckSpawnLimit() && spawnTimer >= spawnDelay)
        {
            if (spawnPoint != null)
            {
                Vector3 spawnPosition = FindGroundPosition(spawnPoint.position);
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                spawnTimer = 0f;
                numberOfEnemiesSpawned++;
            }
            else
            {
                Debug.LogError("Spawn point not assigned!");
            }
        }
    }

    bool CheckSpawnLimit()
    {
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        return currentEnemies < maxNumberOfEnemies && numberOfEnemiesSpawned < maxNumberOfEnemies;
    }

    Vector3 FindGroundPosition(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        else
        {
            Debug.LogWarning("No ground found!");
            return origin;
        }
    }
}