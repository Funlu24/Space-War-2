using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public float spawnRate = 3f; // Kaç saniyede bir meteor düşsün?
    private float nextSpawnTime;

    public float xMin = -8f;
    public float xMax = 8f;
    public float yPos = 7f; 

    // Unity'de ObjectPoolManager'a yazdığın Tag isimleri
    private string[] meteorTags = { "Meteor1", "Meteor2", "Meteor3" };

    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnRandomMeteor();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnRandomMeteor()
    {
        // 1. POZİSYON SEÇ: Rastgele X koordinatı
        float randomX = Random.Range(xMin, xMax);
        Vector3 spawnPos = new Vector3(randomX, yPos, 0);

        // 2. TÜR SEÇ: Listeden rastgele bir meteor ismi çek
        int randomIndex = Random.Range(0, meteorTags.Length);
        string selectedTag = meteorTags[randomIndex];

        // 3. HAVUZDAN ÇAĞIR
        ObjectPool.instance.SpawnFromPool(selectedTag, spawnPos, Quaternion.identity);
    }
}