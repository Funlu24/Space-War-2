using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Oyun Durumu")]
    public bool isGameOver = false;
    public int score = 0;
    public int highScore = 0; // En yüksek skor

    [Header("Level & XP Sistemi")]
    public int currentLevel = 1;
    public float currentXP = 0;
    public float requiredXP = 100; // İlk level için gereken XP
    public float xpMultiplier = 1.2f; // Zorluk artış oranı

    [Header("Spawner Ayarları")]
    // Havuzdaki etiket isimleri (Unity'de yazdığın Tag'ler)
    public string[] enemyTags = { "Enemy1", "Enemy2", "Enemy3" }; 
    public string[] meteorTags = { "Meteor1", "Meteor2", "Meteor3" };
    
    public float minInstantiateValue = -8f;
    public float maxInstantiateValue = 8f;

    [Header("Efektler")]
    public GameObject ParticleEffect;
    public GameObject MuzzleFlashEffect;

    [Header("UI & Paneller")]
    public GameObject StartMenu;
    public GameObject PausePanel;
    public GameObject LevelUpPanel; // Unity'den buraya paneli sürükleyeceksin
    
    public TextMeshProUGUI scoreText;       
    public TextMeshProUGUI bestScoreStart;  
    public TextMeshProUGUI bestScorePause;  
    public TextMeshProUGUI levelText;
    
    public Slider healthBar; // Can Barı
    public Slider xpBar;     // XP Barı

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        // High Score Yükle
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();

        // UI Başlangıç Ayarları
        UpdateUI();
        StartMenu.SetActive(true);
        PausePanel.SetActive(false);
        Time.timeScale = 0f; // Oyun duruk başlar

        // Spawner'ları Başlat (Saniye ayarlarını buradan değiştirebilirsin)
        InvokeRepeating("SpawnEnemy", 1f, 1.5f);
        InvokeRepeating("SpawnAsteroid", 2f, 3f);
    }

    private void Update()
    {
        // ESC Tuşu ile Durdurma
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PausePanel.activeSelf) PauseGameButton(false);
            else PauseGameButton(true);
        }
    }

    // --- SKOR VE XP YÖNETİMİ ---
   public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null) scoreText.text = "Score: " + score;

        // High Score Kontrolü
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            UpdateHighScoreText();
        }

        // Skor kazanınca XP de kazan (Skorun yarısı kadar)
       // GainXP(amount / 2);
    }

    public void GainXP(float amount)
    {
        currentXP += amount;

        // Level Atladı mı?
        if (currentXP >= requiredXP)
        {
            LevelUp();
        }

        UpdateXPBar();
    }

    void LevelUp()
    {
        currentLevel++;
        currentXP = 0; 
        requiredXP *= xpMultiplier; // Sonraki level zorlaşsın

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayLevelUp();
        }

        Time.timeScale = 0f; 

    // 2. Level Up Panelini Aç
    if (LevelUpPanel != null)
    {
        LevelUpPanel.SetActive(true);
    }

        // Level atlama efekti veya sesi buraya eklenebilir
        Debug.Log("LEVEL ATLADIN: " + currentLevel);

        UpdateUI();
    }

    // --- UI GÜNCELLEMELERİ ---
    void UpdateHighScoreText()
    {
        if(bestScoreStart != null) bestScoreStart.text = "Best Score: " + highScore;
        if(bestScorePause != null) bestScorePause.text = "Best: " + highScore;
    }

    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void UpdateXPBar()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = requiredXP;
            xpBar.value = currentXP;
        }
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (levelText != null) levelText.text = "Level: " + currentLevel;
        UpdateXPBar();
    }

    // --- HAVUZ SİSTEMİ İLE SPAWN (Optimize Edildi) ---
    void SpawnEnemy()
    {
        // Menüdeysek üretme
        if (Time.timeScale == 0f) return;

        Vector3 pos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 7f, 0);
        
        // Rastgele bir düşman türü seç (Havuzdan)
        if (enemyTags.Length > 0)
        {
            string randomTag = enemyTags[Random.Range(0, enemyTags.Length)];
            ObjectPool.instance.SpawnFromPool(randomTag, pos, Quaternion.Euler(0, 0, 180f));
        }
    }

    void SpawnAsteroid()
    {
        // Menüdeysek üretme
        if (Time.timeScale == 0f) return;

        Vector3 pos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 8f, 0);

        // Rastgele bir meteor türü seç (Havuzdan)
        if (meteorTags.Length > 0)
        {
            string randomTag = meteorTags[Random.Range(0, meteorTags.Length)];
            ObjectPool.instance.SpawnFromPool(randomTag, pos, Quaternion.identity);
        }
    }
    public void CloseLevelUpPanel()
{
    if (LevelUpPanel != null)
    {
        LevelUpPanel.SetActive(false);
    }

    // Oyunu tekrar hareket ettir
    Time.timeScale = 1f; 
}

    // --- BUTON FONKSİYONLARI ---
    public void StartGameButton()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            StartMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void PauseGameButton(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            PausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            PausePanel.SetActive(false);
        }
    }

    public void GameOver()
    {
        StartMenu.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Oyun Bitti.");
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}