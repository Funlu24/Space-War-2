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
    public bool canShoot = false; // BU KUTUYU İŞARETLERSEN ATEŞ EDER
    public GameObject enemyBulletPrefab; // Düşman mermisi prefabını buraya sürükle
    public float fireRate = 2f; // Kaç saniyede bir ateş etsin
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

        startPos = transform.position;
        
        // Oyun başladığında hemen ateş etmesin, biraz rastgele beklesin
        nextFireTime = Time.time + Random.Range(0.5f, 2f);
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
        // Eğer "canShoot" işaretliyse VE zamanı geldiyse ateş et
        if (canShoot && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
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
        pos.x = startPos.x + Mathf.Sin(Time.time * frequency) * magnitude;
        transform.position = pos;
    }

    void Shoot()
    {
        if (enemyBulletPrefab != null)
        {
            // Mermiyi oluştur
            Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
        }
    }
}