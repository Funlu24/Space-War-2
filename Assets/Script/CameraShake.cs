using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    private Vector3 originalPos; // Kameranın orijinal yerini saklayacağız

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    // Sallantıyı başlatan fonksiyon
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Rastgele bir pozisyon üret (X ve Y ekseninde titret)
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Kamerayı oynat
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            // Bir sonraki kareyi bekle
            yield return null;
        }

        // Süre bitince kamerayı eski yerine koy
        transform.localPosition = originalPos;
    }
}