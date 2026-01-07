using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public float lifeTime = 4f; // Ekranda kalma süresi

    void Start()
    {
        // Bu obje (Tutorial Paneli), oyun zamanı işlemeye başladığında
        // belirtilen saniye kadar sonra kendini yok eder.
        
        // Not: Oyun başında Time.timeScale = 0 olduğu için,
        // Start butonuna basana kadar süre azalmaz, ekranda durur.
        // Start'a bastığın an geri sayım başlar.
        
        Destroy(gameObject, lifeTime);
    }
}