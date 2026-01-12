using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnRate = 2f; 
    private float nextSpawnTime;

    public float xMin = -8f;
    public float xMax = 8f;
    public float yPos = 6f; 

    // Unity'de yazdığın Tag isimlerini buraya liste olarak yazıyoruz
    private string[] enemyTags = { "Enemy1", "Enemy2", "Enemy3" };

    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnRandomEnemy(); // İsmini değiştirdim
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnRandomEnemy()
    {
        float randomX = Random.Range(xMin, xMax);
        Vector3 spawnPos = new Vector3(randomX, yPos, 0);

        // 1. RASTGELE SEÇİM: Listeden rastgele bir etiket seç
        int randomIndex = Random.Range(0, enemyTags.Length);
        string selectedTag = enemyTags[randomIndex];

        // 2. HAVUZDAN ÇAĞIR: Seçilen etiketi havuza sor
        ObjectPool.instance.SpawnFromPool(selectedTag, spawnPos, Quaternion.identity);
    }
}