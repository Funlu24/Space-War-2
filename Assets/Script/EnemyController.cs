using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { DuzGiden, TakipEden, ZikzakGiden }

public class EnemyController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public EnemyType dusmanTuru;
    public float speed = 4f; 
    
    [Header("Ateş Etme Ayarları")]
    public bool canShoot = false; 
    public GameObject enemyBulletPrefab; 
    public float fireRate = 2f; 
    private float nextFireTime;

    [Header("Zikzak Ayarları")]
    public float frequency = 2f; 
    public float magnitude = 2f; 

    private Transform playerTransform;
    private Vector3 startPos; 

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    // HAVUZ SİSTEMİ İÇİN KRİTİK: Düşman her canlandığında burası çalışır
    void OnEnable()
    {
        startPos = transform.position; // Yeni doğduğu yeri kaydet (Zikzak için şart)
        nextFireTime = Time.time + Random.Range(0.5f, 2f); // Ateş süresini sıfırla
    }

    void Update()
    {
        // 1. HAREKET MANTIĞI
        switch (dusmanTuru)
        {
            case EnemyType.DuzGiden:
                MoveStraight();
                break;
            case EnemyType.TakipEden:
                MoveTowardsPlayer();
                break;
            case EnemyType.ZikzakGiden:
                MoveZigZag();
                break;
        }

        // 2. ATEŞ ETME MANTIĞI
        if (canShoot && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // 3. EKRANDAN ÇIKMA MANTIĞI
        if (transform.position.y < -6f)
        {
            gameObject.SetActive(false); 
        }
    }

    void MoveStraight()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
    }

    void MoveTowardsPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        else
        {
            MoveStraight();
        }
    }

    void MoveZigZag()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
        Vector3 pos = transform.position;
        // startPos.x OnEnable içinde güncellendiği için artık düzgün çalışır
        pos.x = startPos.x + Mathf.Sin(Time.time * frequency) * magnitude;
        transform.position = pos;
    }

    void Shoot()
    {
        // Object Pooling ile mermi oluşturma
        ObjectPool.instance.SpawnFromPool("EnemyBullet", transform.position, Quaternion.identity);
    }

    // --- İŞTE SENİN EKLEMEN GEREKEN YER BURASI ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. OYUNCUYA ÇARPARSA
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerConroller player = collision.gameObject.GetComponent<PlayerConroller>();
            if (player != null)
            {
                player.TakeDamage(); // Oyuncunun canını azalt
            }
            gameObject.SetActive(false); // Düşmanı yok et (XP vermez, çünkü sen çarptın)
        }
        
        // 2. MERMİYE ÇARPARSA (Senin istediğin kısım)
        else if (collision.gameObject.CompareTag("Missile")) 
        {
            // --- XP KÜRESİ OLUŞTUR ---
            ObjectPool.instance.SpawnFromPool("XPOrb", transform.position, Quaternion.identity);
            
            collision.gameObject.SetActive(false); // Mermiyi kapat
            GameManager.instance.AddScore(100);    // Skor ver
            gameObject.SetActive(false);           // Düşmanı kapat
        }
    }
}