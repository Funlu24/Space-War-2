using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; // <-- BU KÜTÜPHANEYİ EKLEDİK (TextMeshPro için şart)
using UnityEngine.SceneManagement; // <-- Sahne yönetimi için şart!

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Enemy Spawning")]
    public GameObject[] EnemyPrefabs;
    public float minInstantiateValue;
    public float maxInstantiateValue;
    public float enemyDestroyTime = 10f;

    [Header("Effects")]
    public GameObject ParticleEffect;
    public GameObject MuzzleFlashEffect;

    [Header("UI & Panels")]
    public GameObject StartMenu;
    public GameObject PausePanel;
    [Header("Asteroid Spawning")]
public GameObject[] AsteroidPrefabs; // Meteor çeşitleri
    
    // --- BURAYI DEĞİŞTİRDİK ---
    // Artık "Text" değil "TextMeshProUGUI" istiyoruz.
    public TextMeshProUGUI scoreText;       
    public TextMeshProUGUI bestScoreStart;  
    public TextMeshProUGUI bestScorePause;  

    // SKOR DEĞİŞKENLERİ
    int score = 0;
    int highScore = 0;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();

        StartMenu.SetActive(true);
        PausePanel.SetActive(false);
        Time.timeScale = 0f;
        
        InvokeRepeating("InstantiateEnemy", 1f, 1f);
        InvokeRepeating("InstantiateAsteroid", 2f, 3f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PausePanel.activeSelf) PauseGameButton(false);
            else PauseGameButton(true);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            UpdateHighScoreText();
        }
    }

    void UpdateHighScoreText()
    {
        if(bestScoreStart != null) bestScoreStart.text = "Best Score: " + highScore;
        if(bestScorePause != null) bestScorePause.text = "Best: " + highScore;
    }

    void InstantiateEnemy()
    {
        Vector3 Enemypos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f);
        int randomIndex = Random.Range(0, EnemyPrefabs.Length); 
        GameObject enemy = Instantiate(EnemyPrefabs[randomIndex], Enemypos, Quaternion.Euler(0, 0, 180f));
        Destroy(enemy, enemyDestroyTime);
    }
    void InstantiateAsteroid()
{
    // Oyun duraklatıldıysa veya menüdeysek üretme
    if (Time.timeScale == 0f) return;

    Vector3 asteroidPos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 7f);

    // Rastgele bir meteor seç
    int randomIndex = Random.Range(0, AsteroidPrefabs.Length);

    // Oluştur
    GameObject asteroid = Instantiate(AsteroidPrefabs[randomIndex], asteroidPos, Quaternion.identity);

    // 10 saniye sonra yok et (Ekranı doldurmasın)
    Destroy(asteroid, 10f);
}

    public void StartGameButton()
    {
        // Kontrol Ediyoruz: Sahnede "Player" etiketli bir obje var mı?
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            // OYUNCU YAŞIYOR (İlk açılış veya Pause'dan devam)
            StartMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            // OYUNCU ÖLMÜŞ (Destroy olmuş)
            // Sahneyi baştan yükle (Reset at)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
            // NOT: Sahne yeniden yüklenince oyunun 'Start' fonksiyonu çalışacak 
            // ve menü otomatik olarak tekrar açılacak. Bu normaldir.
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
        Debug.Log("Oyundan çıkış yapıldı!"); // Konsolda çalıştığını görmek için

        // Gerçek oyunda (Build alındığında) uygulamayı kapatır
        Application.Quit();

        // Unity Editöründe test ederken "Play" modunu durdurur
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}