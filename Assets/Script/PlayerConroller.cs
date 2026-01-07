using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConroller : MonoBehaviour
{
    [Header("Movement & Health")]
    public float moveSpeed = 10f;
    public int health = 3; 
    private bool isInvulnerable = false; 

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

    [Header("Components & Effects")]
    public Animator shipAnimator;          
    public GameObject EngineThrustEffect;  
    private SpriteRenderer spriteRenderer; 

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 1. ADIM: Girdileri (Input) burada alıyoruz.
    // Update her kare çalışır, tuş basımını kaçırmaz.
    private void Update()
    {
        ProcessInputs(); // Inputları alan fonksiyon
        PlayerShoot();
        CheckEnemyAhead();
    }

    // [GEREKSİNİM 6 - KISIM A] FixedUpdate
    // 2. ADIM: Hareketi burada uyguluyoruz.
    // Fizik ve hareket hesaplamaları için sabit zaman aralığında çalışır (0.02sn).
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // [GEREKSİNİM 6 - KISIM B] LateUpdate
    // 3. ADIM: Her şey bittikten sonra sınırları düzeltiyoruz.
    private void LateUpdate()
    {
        ClampPosition();
    }

    void ProcessInputs()
    {
        // Tuş verilerini alıp değişkenlere kaydediyoruz
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        // Motor Efekt Kontrolü (Görsel olduğu için Update/ProcessInput içinde kalabilir)
        if (EngineThrustEffect != null)
        {
            if (moveZ > 0) EngineThrustEffect.SetActive(true);
            else EngineThrustEffect.SetActive(false);
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
        // FixedUpdate içinde olduğumuz için Time.deltaTime kullanmak yine güvenlidir
        // ama Unity burayı sabit aralıklarla çağırır.
        if (direction.magnitude > 0)
        {
            // .normalized diyerek vektörün boyunu 1'e sabitliyoruz.
            // Böylece çapraz giderken hızlanmıyor.
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
        RaycastHit2D hit = Physics2D.Raycast(MissileSpawnPoint.position, Vector2.up, 10f);
        Debug.DrawRay(MissileSpawnPoint.position, Vector3.up * 10f, Color.red);
    }

    void PlayerShoot()
    {
        if (Input.GetButtonDown("Jump"))
        {
            SpawnMissile();
            SpawnMuzzleFlash();
        }
    }

    void SpawnMissile()
    {
        GameObject gm = Instantiate(MissiliePrefab, MissileSpawnPoint.position, Quaternion.identity);
        gm.transform.SetParent(null);
        Destroy(gm, DestroyTime);
    }

    void SpawnMuzzleFlash()
    {
        if (GameManager.instance.MuzzleFlashEffect != null)
        {
            GameObject muzzle = Instantiate(GameManager.instance.MuzzleFlashEffect, MissileSpawnPoint.position, Quaternion.identity);
            muzzle.transform.SetParent(null);
            Destroy(muzzle, DestroyTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isInvulnerable) return;

            GameObject impactEffect = Instantiate(GameManager.instance.ParticleEffect, transform.position, Quaternion.identity);
            Destroy(impactEffect, 2f);
            Destroy(collision.gameObject);

            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        health--; 

        if (health >= 0 && health < heartIcons.Length)
        {
            heartIcons[health].SetActive(false);
        }

        if (health <= 0)
        {
            GameManager.instance.GameOver(); 
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
}