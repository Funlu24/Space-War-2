using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    public float lifeTime = 3f; // Kaç saniye sonra havuza dönsün?

    // Obje her aktif olduğunda (Spawn edildiğinde) bu çalışır
    void OnEnable()
    {
        // Belirtilen süre sonra "Hide" fonksiyonunu çalıştır
        Invoke("Hide", lifeTime);
    }

    void Hide()
    {
        // Destroy yerine SetActive(false) kullanıyoruz
        gameObject.SetActive(false); 
    }
    
    // Eğer mermi süre dolmadan bir şeye çarparsa bu fonksiyon çağrılır
    // (Bunu birazdan PlayerController ve Enemy scriptlerinde kullanacağız)
    public void ReturnNow()
    {
        CancelInvoke(); // Sayacı durdur
        gameObject.SetActive(false);
    }
}