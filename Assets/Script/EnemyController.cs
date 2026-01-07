using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f; // Düşman hızı
    private Transform playerTransform; // Oyuncunun yerini tutacak değişken

    void Start()
    {
        // Oyun başladığında sahnede "Player" etiketli objeyi bul ve yerini hafızaya al
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // Eğer oyuncu hala hayattaysa
        if (playerTransform != null)
        {
            // 1. YÖN HESAPLA: (Hedef - Ben) işlemi yönü verir
            Vector3 direction = (playerTransform.position - transform.position).normalized;

            // 2. O YÖNE GİT
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // 3. YÜZÜNÜ DÖN (Opsiyonel ama şık durur)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        else
        {
            // Oyuncu öldüyse dümdüz aşağı devam et
            transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
        }
    }
    
    // Çarpışma kodlarını kaldırdım çünkü senin mermi scriptin (MissileController)
    // zaten çarpışmayı yönetip düşmanı yok ediyor. İki tarafta da kod olursa hata çıkar.
}