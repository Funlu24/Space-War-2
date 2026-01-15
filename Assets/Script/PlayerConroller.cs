using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConroller : MonoBehaviour
{
    [Header("Movement & Health")]
    public float moveSpeed = 10f;
    public int health = 3; 
    private bool isInvulnerable = false; 
    public int maxHealth = 100;

    // Hareket girişlerini (Input) burada tutacağız
    private float moveX;
    private float moveZ;

    [Header("UI Elements")]
    public GameObject[] heartIcons; 

    [Header("Missile")]
    public GameObject MissiliePrefab; // (ObjectPool kullandığımız için bu boş kalabilir)
    public Transform MuzzleSpawnPosition;
    public float DestroyTime = 5f;
    public Transform MissileSpawnPoint;
    
    public float fireRate = 0.2f;    // Ateş etme sıklığı (Saniye)
    private float nextFireTime = 0f; // Zamanlayıcı
    private bool isAutoFiring = false; // Başlangıçta kapalı olsun

    [Header("Components & Effects")]
    public Animator shipAnimator;          
    public GameObject EngineThrustEffect;  // Eski sprite efekti (varsa kalsın)
    
    // --- YENİ EKLENEN KISIM ---
    public ParticleSystem engineTrail;     // Yeni Motor İzi Efekti
    // --------------------------

    private SpriteRenderer spriteRenderer; 
    

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = maxHealth; 
        
        // GameManager'a "Canımı fulle" de
        GameManager.instance.UpdateHealthUI(health, maxHealth);

        // Başlangıçta motor izini kapatalım (Tuşa basınca açılsın)
        if(engineTrail != null)
        {
            var emission = engineTrail.emission;
            emission.enabled = false;
        }
    }

   
    private void Update()
    {
        ProcessInputs(); // Inputları alan fonksiyon
        PlayerShoot();
        CheckEnemyAhead();
    }

    
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void LateUpdate()
    {
        ClampPosition();
    }

    void ProcessInputs()
    {
        // Tuş verilerini alıp değişkenlere kaydediyoruz
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        // 1. Eski Motor Efekt Kontrolü (GameObject olan)
        if (EngineThrustEffect != null)
        {
            if (moveZ > 0) EngineThrustEffect.SetActive(true);
            else EngineThrustEffect.SetActive(false);
        }

        // --- 2. YENİ PARTICLE SYSTEM KONTROLÜ ---
        // Sadece ileri (W veya Yukarı) giderken iz çıksın
        if (engineTrail != null)
        {
            var emission = engineTrail.emission;
            
            if (moveZ > 0) // Gaza basılıyorsa
            {
                emission.enabled = true;
            }
            else // Duruyor veya geri gidiyorsa
            {
                emission.enabled = false;
            }
        }
        // ----------------------------------------

        // Animasyon Kontrolü
        if (shipAnimator != null)
        {
            shipAnimator.SetBool("IsMoving", moveX != 0 || moveZ != 0);
        }
    
    }

    void MovePlayer()
    {
        Vector3 direction = new Vector3(moveX, moveZ, 0);
        
        if (direction.magnitude > 0)
        {
            direction = direction.normalized;
        }
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    void ClampPosition()
    {
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, -9.0f, 9.0f);
        viewPos.y = Mathf.Clamp(viewPos.y, -4.5f, 4.5f);
        transform.position = viewPos;
    }

    void CheckEnemyAhead()
    {
        // Debug amaçlı, oyun içinde görünmez
        Debug.DrawRay(MissileSpawnPoint.position, Vector3.up * 10f, Color.red);
    }

    void PlayerShoot()
    {
        // 1. AÇMA / KAPAMA KONTROLÜ
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            isAutoFiring = !isAutoFiring; 
        }

        // 2. ATEŞ ETME EYLEMİ
        if (isAutoFiring && Time.time > nextFireTime)
        {
            SpawnMissile();
            SpawnMuzzleFlash();
            nextFireTime = Time.time + fireRate;
        }
    }

    void SpawnMissile()
    {
        ObjectPool.instance.SpawnFromPool("PlayerBullet", MissileSpawnPoint.position, Quaternion.identity);

        // Ses Efekti (Hata vermemesi için kontrol ekli)
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlayShoot(); 
        }
    }

    void SpawnMuzzleFlash()
    {
        ObjectPool.instance.SpawnFromPool("MuzzleFlash", MissileSpawnPoint.position, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isInvulnerable) return;

            // Çarpışma Efekti
            if(GameManager.instance.ParticleEffect != null)
            {
                GameObject impactEffect = Instantiate(GameManager.instance.ParticleEffect, transform.position, Quaternion.identity);
                Destroy(impactEffect, 2f);
            }

            collision.gameObject.SetActive(false);
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (isInvulnerable) return;

        health -= 10; 

        // GameManager'daki barı güncelle
        GameManager.instance.UpdateHealthUI(health, maxHealth);
        
        // Ekranı salla
        if (CameraShake.instance != null)
        {
            CameraShake.instance.Shake(0.2f, 0.4f); 
        }

        // Can 0 olursa öl
        if (health <= 0)
        {
            GameManager.instance.GameOver(); 
            // Destroy(gameObject) yerine SetActive(false) kullanmak daha güvenli olabilir
            // ama şimdilik senin kodunu korudum.
            Destroy(gameObject); 
        }
        else
        {
            StartCoroutine(BlinkRoutine());
        }
    }

    IEnumerator BlinkRoutine()
    {
        isInvulnerable = true; 
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false; 
            yield return new WaitForSeconds(0.1f); 
            spriteRenderer.enabled = true; 
            yield return new WaitForSeconds(0.1f); 
        }
        isInvulnerable = false; 
    }

    // --- GÜÇLENDİRME (UPGRADE) FONKSİYONLARI ---
    
    public void UpgradeFireRate()
    {
        if (fireRate > 0.15f) 
        {
            fireRate -= 0.05f; 
            Debug.Log("Atış Hızı Arttı! Yeni Hız: " + fireRate);
        }
        else
        {
            Debug.Log("MAKSİMUM HIZA ULAŞILDI!");
        }
    }

    public void UpgradeSpeed()
    {
        moveSpeed += 2f;
        Debug.Log("Hız Arttı!");
    }

    public void UpgradeHealth()
    {
        maxHealth += 10; // Can upgrade alınca max can artsın (Seninkinde 1'di 10 yaptım daha mantıklı olsun diye)
        health = maxHealth; 
        GameManager.instance.UpdateHealthUI(health, maxHealth); 
        Debug.Log("Can Fullendi ve Arttı!");
    }
}