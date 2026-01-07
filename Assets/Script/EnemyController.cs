using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Düşman türlerini burada listeliyoruz
public enum EnemyType { DuzGiden, TakipEden, ZikzakGiden }

public class EnemyController : MonoBehaviour
{
    [Header("Ayarlar")]
    public EnemyType dusmanTuru; // Unity'den seçeceğimiz kutucuk
    public float speed = 4f; 
    
    [Header("Zikzak Ayarları (Sadece Zikzak seçilirse çalışır)")]
    public float frequency = 2f; // Ne kadar hızlı sağ-sol yapsın
    public float magnitude = 2f; // Ne kadar geniş sağ-sol yapsın

    private Transform playerTransform;
    private Vector3 startPos; // Zikzak için başlangıç pozisyonu

    void Start()
    {
        // Oyuncuyu bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        startPos = transform.position; // Doğduğu yeri kaydet
    }

    void Update()
    {
        // Hangi tür seçiliyse o hareketi yap
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
    }

    // 1. TİP: DÜZ AŞAĞI (Klasik)
    void MoveStraight()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
    }

    // 2. TİP: TAKİPÇİ (Kamikaze)
    void MoveTowardsPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
            
            // Yüzünü dönme efekti
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        else
        {
            MoveStraight(); // Oyuncu yoksa düz git
        }
    }

    // 3. TİP: ZİKZAK (Yılan gibi)
    void MoveZigZag()
    {
        // Aşağı inmeye devam et
        transform.position += Vector3.down * speed * Time.deltaTime;

        // Sağa sola sinüs dalgası ekle
        Vector3 pos = transform.position;
        // Mathf.Sin ile dalgalanma yaratıyoruz
        pos.x = startPos.x + Mathf.Sin(Time.time * frequency) * magnitude;
        transform.position = pos;
    }
}