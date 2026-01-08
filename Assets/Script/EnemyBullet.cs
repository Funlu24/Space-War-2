using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f; // Mermi hızı
    public float lifeTime = 3f; // Kaç saniye sonra yok olsun

    void Start()
    {
        Destroy(gameObject, lifeTime); // Mermi sonsuza kadar gitmesin, süre bitince yok olsun
    }

    void Update()
    {
        // Mermi aşağı doğru (Vector3.down) gider
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer çarptığımız şey "Player" ise
        if (collision.CompareTag("Player"))
        {
            // Oyuncunun scriptini bul
            PlayerConroller player = collision.GetComponent<PlayerConroller>();
            
            if (player != null)
            {
                player.TakeDamage(); // Oyuncunun canını azalt
            }

            // Mermiyi yok et
            Destroy(gameObject);
        }
        // Eğer mermi başka bir düşmana veya kendi mermisine çarparsa hiçbir şey yapma
    }
}