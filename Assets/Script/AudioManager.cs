using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Ses Klipleri")]
    public AudioClip shootClip;
    public AudioClip explosionClip;
    public AudioClip crashClip;
    public AudioClip xpClip;
    public AudioClip levelUpClip;

    [Header("Ses Kaynağı")]
    public AudioSource sfxSource;     // Efektler için (Pew, Boom)
    public AudioSource musicSource;   // Arka plan müziği için

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Tek seferlik ses çalma fonksiyonu
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            // PlayOneShot: Üst üste ses çalmaya izin verir (Seri ateş gibi)
            sfxSource.PlayOneShot(clip);
        }
    }

    // Özel Fonksiyonlar (Diğer scriptlerden çağırmak kolay olsun)
    public void PlayShoot() => PlaySFX(shootClip);
    public void PlayExplosion() => PlaySFX(explosionClip);
    public void PlayXP() => PlaySFX(xpClip);
    public void PlayLevelUp() => PlaySFX(levelUpClip);
    public void PlayCrash() => PlaySFX(crashClip);
}