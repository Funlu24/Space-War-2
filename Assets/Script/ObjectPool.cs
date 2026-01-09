using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Singleton yapısı: Her yerden ulaşabilmek için
    public static ObjectPool instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;        // Havuzun adı (Örn: "PlayerBullet")
        public GameObject prefab; // Hangi objeyi üreteceğiz?
        public int size;          // Kaç tane üretelim?
    }

    public List<Pool> pools; // Unity Editöründen ayarlanacak liste
    public Dictionary<string, Queue<GameObject>> poolDictionary; // Arka planda çalışan sistem

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Tüm havuzları baştan oluşturuyoruz
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false); // Başta kapalı olsun
                objectPool.Enqueue(obj); // Kuyruğa ekle
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Havuzda " + tag + " etiketi bulunamadı!");
            return null;
        }

        // --- KORUMA BURADA BAŞLIYOR ---
        // Eğer havuzda hiç obje kalmadıysa (Queue Empty hatasını önler)
        if (poolDictionary[tag].Count == 0)
        {
            Debug.LogWarning(tag + " havuzu tükendi! Size miktarını arttırın.");
            return null; // Oyunun çökmesini engeller, sadece ateş etmezsin.
        }
        // --- KORUMA BİTTİ ---

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // --- İKİNCİ KORUMA (MissingReference hatasını önler) ---
        // Eğer obje dışarıdan bir şekilde silindiyse (Destroy edildiyse), yenisini yaratmazsak hata alırız.
        if (objectToSpawn == null) 
        {
             return null; 
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}