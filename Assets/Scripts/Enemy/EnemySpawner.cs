using Pathfinding.Util;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] protected string enemyName = "";
    [SerializeField] protected int spawnLimit = 2;
    public GameObject enemyPrefab;


    public float spawnRate = 1f; // Tần suất xuất hiện của quái vật
    public int numberOfEnemies = 3; // Số lượng quái vật cần sinh ra

    private float spawnTimer = 0f;

    public float spawnRadius = 10f; // Bán kính khu vực xuất hiện

    void Start()
    {
        SpawnEnemies();
    }

    void Update()
    {
        // Tăng hẹn giờ spawn
        spawnTimer += Time.deltaTime;

        // Kiểm tra nếu đến lúc tạo quái vật mới
        if (spawnTimer >= spawnRate)
        {
            SpawnEnemy();
            spawnTimer = 0f; // Reset hẹn giờ
        }
    }

protected virtual void Spawning()
    {
        Invoke("Spawning", 2);
        if (!this.CanSpawn()) return;
        float x = Random.Range(-7f, 7f);
        float y = Random.Range(3f, 6f);
        Vector2 spawnPos = new Vector2(x, y);
/*        Transform obj = ObjectPool.Spawn(this.enemyName, spawnPos, transform.rotation, transform);
        obj.gameObject.SetActive(true);*/
    }

    protected virtual bool CanSpawn()
    {
        int childCount = transform.childCount;
        Debug.Log(childCount);
        if (childCount >= this.spawnLimit) return false;
        return true;
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // Tạo vị trí spawn ngẫu nhiên trong bán kính spawnRadius
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

        // Instantiate quái vật tại vị trí spawn ngẫu nhiên
        Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
    }
}
