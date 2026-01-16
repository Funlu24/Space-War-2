using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConroller : MonoBehaviour
{
    [Header("Movement & Health")]
    public float moveSpeed = 10f;
    public int health = 3; 
    public int maxHealth = 100;
    
    // --- DASH (ATILMA) AYARLARI ---
    [Header("Dash Settings")]
    public float dashSpeed = 25f;        // Dash hızı (Normalden hızlı olmalı)
    public float dashDuration = 0.2f;    // Atılma süresi (Kısa olmalı)
    public float dashCooldown = 1.5f;    // Tekrar basabilmek için bekleme süresi
    public float invulnerabilityTime = 1.5f; // Hasar almama süresi
    
    private bool isDashing = false;      // Şu an Dash atıyor mu?
    private bool isInvulnerable = false; // Şu an dokunulmaz mı?
    private float nextDashTime = 0f;     // Dash zamanlayıcısı
    // -----------------------------

    // Hareket girişlerini (Input) burada tutacağız
    private float moveX;
    private float moveZ;

    [Header("UI Elements")]
    public GameObject[] heartIcons; 

    [Header("Missile")]
    public GameObject MissiliePrefab; 
    public Transform MuzzleSpawnPosition;
    public float DestroyTime = 5f;
    public Transform MissileSpawnPoint;
    
    public float fireRate = 0.2f;    
    private float nextFireTime = 0f; 
    private bool isAutoFiring = false; 

    [Header("Components & Effects")]
    public Animator shipAnimator;          
    public GameObject EngineThrustEffect;  
    public ParticleSystem engineTrail;     // Yeni Motor İzi Efekti

    private SpriteRenderer spriteRenderer; 
    

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = maxHealth; 
        
        GameManager.instance.UpdateHealthUI(health, maxHealth);

        // Başlangıçta motor izini kapatalım
        if(engineTrail != null)
        {
            var emission = engineTrail.emission;
            emission.enabled = false;
        }
    }

   
    private void Update()
    {
        ProcessInputs(); 
        PlayerShoot();
        CheckEnemyAhead();

        // --- DASH KONTROLÜ (YENİ) ---
        // Left Shift'e basıldıysa, cooldown dolduysa ve şu an dash atmıyorsa
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
        // ----------------------------
    }

    
    private void FixedUpdate()
    {
        // Dash atarken normal hareket kodunu çalıştırma, çakışmasın
        if (!isDashing)
        {
            MovePlayer();
        }
    }

    private void LateUpdate()
    {
        ClampPosition();
    }

    void ProcessInputs()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        // 1. Eski Motor Efekt Kontrolü
        if (EngineThrustEffect != null)
        {
            if (moveZ > 0) EngineThrustEffect.SetActive(true);
            else EngineThrustEffect.SetActive(false);
        }

        // 2. PARTICLE SYSTEM KONTROLÜ
        // Eğer Dash atıyorsak burası karışmasın, DashRoutine yönetecek
        if (engineTrail != null && !isDashing)
        {
            var emission = engineTrail.emission;
            
            if (moveZ > 0) // Gaza basılıyorsa
            {
                emission.enabled = true;
            }
            else // Duruyorsa
            {
                emission.enabled = false;
            }
        }

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

    // --- YENİ EKLENEN DASH FONKSİYONU ---
    IEnumerator DashRoutine()
    {
        isDashing = true;
        isInvulnerable = true; // Hasar almazlık başlasın
        nextDashTime = Time.time + dashCooldown; // Cooldown ayarla

        // 1. Dash yönünü belirle (Tuşlara basılmıyorsa ileri git)
        Vector3 dashDir = new Vector3(moveX, moveZ, 0).normalized;
        if (dashDir == Vector3.zero) dashDir = Vector3.up;

        // 2. Motor İzini Zorla Aç (Görsellik için)
        if (engineTrail != null)
        {
            var emission = engineTrail.emission;
            emission.enabled = true;
        }

        // 3. Yanıp Sönmeyi Başlat (Dokunulmazlık süresi kadar)
        StartCoroutine(BlinkRoutine(invulnerabilityTime));

        // 4. Fiziksel Atılma Hareketi
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            // Normal hız yerine dashSpeed kullanıyoruz
            transform.Translate(dashDir * dashSpeed * Time.deltaTime);
            ClampPosition(); // Dash atarken ekrandan çıkmasın
            yield return null;
        }

        // 5. Dash Hareketi Bitti
        isDashing = false;

        // Eğer oyuncu "W" tuşuna basmıyorsa izi kapat
        if (engineTrail != null && moveZ <= 0)
        {
            var emission = engineTrail.emission;
            emission.enabled = false;
        }

        // Dokunulmazlık süresinin geri kalanını bekle
        yield return new WaitForSeconds(invulnerabilityTime - dashDuration);
        
        isInvulnerable = false; // Artık hasar alabilir
    }
    // ------------------------------------

    void ClampPosition()
    {
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, -9.0f, 9.0f);
        viewPos.y = Mathf.Clamp(viewPos.y, -4.5f, 4.5f);
        transform.position = viewPos;
    }

    void CheckEnemyAhead()
    {
        Debug.DrawRay(MissileSpawnPoint.position, Vector3.up * 10f, Color.red);
    }

    void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            isAutoFiring = !isAutoFiring; 
        }

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
            // Eğer Dash attıysak veya dokunulmazsak HASAR ALMA
            if (isInvulnerable) return;

            if(GameManager.instance.ParticleEffect != null)
            {
                GameObject impactEffect = Instantiate(GameManager.instance.ParticleEffect, transform.position, Quaternion.identity);
                Destroy(impactEffect, 2f);
            }

            collision.gameObject.SetActive(false);
            TakeDamage();
        }
    }

    // Mermiler trigger ise burası da lazım olabilir
    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (isInvulnerable) return; // Dokunulmazken mermiler işlemesin
    }

    public void TakeDamage()
    {
        if (isInvulnerable) return;

        health -= 10; 
        GameManager.instance.UpdateHealthUI(health, maxHealth);
        
        if (CameraShake.instance != null)
        {
            CameraShake.instance.Shake(0.2f, 0.4f); 
        }

        // Hasar alınca kısa bir dokunulmazlık (1 saniye)
        StartCoroutine(BlinkRoutine(1f)); 

        if (health <= 0)
        {
            GameManager.instance.GameOver(); 
            Destroy(gameObject); 
        }
    }

    // BlinkRoutine güncellendi: Artık süre parametresi alıyor
    IEnumerator BlinkRoutine(float duration)
    {
        isInvulnerable = true; 
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            spriteRenderer.enabled = false; 
            yield return new WaitForSeconds(0.1f); 
            spriteRenderer.enabled = true; 
            yield return new WaitForSeconds(0.1f); 
        }
        
        spriteRenderer.enabled = true;
        // Not: isInvulnerable false yapmıyoruz, onu çağıran yer (Dash veya TakeDamage) yönetiyor
        if(!isDashing) isInvulnerable = false;
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
        maxHealth += 10; 
        health = maxHealth; 
        GameManager.instance.UpdateHealthUI(health, maxHealth); 
        Debug.Log("Can Fullendi ve Arttı!");
    }
}