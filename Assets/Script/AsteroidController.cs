using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed = 4f; 
    public float rotationSpeed = 50f; 

    void Update()
    {
        // Hareket ve Dönme
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // --- YENİ: EKRANDAN ÇIKMA KONTROLÜ ---
        // Eğer meteor çok aşağı inerse (kimse vurmadıysa) havuza geri dönsün
        if (transform.position.y < -7f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Oyuncuya Çarparsa
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerConroller playerScript = collision.gameObject.GetComponent<PlayerConroller>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(); 
            }
            if (AudioManager.instance != null)
            {
                // Düşmanla çarpışınca çalan sesin aynısı çalsın
                AudioManager.instance.PlayCrash(); 
            }
            DestroyAsteroid();
        }
        // Mermiye Çarparsa
        else if (collision.gameObject.CompareTag("Missile")) 
        {
            ObjectPool.instance.SpawnFromPool("XPOrb", transform.position, Quaternion.identity);
            AudioManager.instance.PlayExplosion();
            
            // Mermiyi kapat (Havuz mantığı)
            collision.gameObject.SetActive(false); 
            
            GameManager.instance.AddScore(50); 
            DestroyAsteroid(); 
        }
    }

    void DestroyAsteroid()
    {

        if (CameraShake.instance != null)
    {
        CameraShake.instance.Shake(0.2f, 0.3f);
    }

        //AudioManager.instance.PlayExplosion();
        // Patlama efekti (Bunu şimdilik Instantiate bırakabiliriz, ilerde havuza alınabilir)
        if(GameManager.instance.ParticleEffect != null)
        {
            GameObject effect = Instantiate(GameManager.instance.ParticleEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // ESKİ: Destroy(gameObject);
        // YENİ: Havuza geri gönder
        gameObject.SetActive(false); 
    }
}