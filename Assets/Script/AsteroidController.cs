using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed = 4f;           // Düşme hızı
    public float rotationSpeed = 50f;  // Dönme hızı (Görsellik için)

    void Update()
    {
        // DÜZELTME: Space.World ekledik. 
        // Böylece taş dönse bile yönü hep ekranın aşağısı olur.
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        // Kendi etrafında dönme
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    // Çarpışma Mantığı
  private void OnCollisionEnter2D(Collision2D collision)
    {
        // Oyuncuya Çarparsa
        if (collision.gameObject.CompareTag("Player"))
        {
            // 1. Çarptığımız objenin (Player) scriptine ulaş
            PlayerConroller playerScript = collision.gameObject.GetComponent<PlayerConroller>();
            
            // 2. Eğer scripti bulduysak hasar ver
            if (playerScript != null)
            {
                playerScript.TakeDamage(); // Canı 1 azalt
            }

            // 3. Meteoru yok et
            DestroyAsteroid();
        }
        // Mermiye Çarparsa
        else if (collision.gameObject.CompareTag("Missile")) 
        {
            Destroy(collision.gameObject); // Mermiyi yok et
            GameManager.instance.AddScore(50); // (İstersen) Meteoru vurunca 50 puan ver
            DestroyAsteroid(); // Meteoru yok et
        }
    }

    void DestroyAsteroid()
    {
        if(GameManager.instance.ParticleEffect != null)
        {
            GameObject effect = Instantiate(GameManager.instance.ParticleEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        Destroy(gameObject);
    }

}