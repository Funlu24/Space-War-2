using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public float xpAmount = 10f; // Bu küre kaç XP verecek?

   private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. İspiyoncu: Küreye bir şey çarptı mı?
        Debug.Log("Küreye çarpan cisim: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            AudioManager.instance.PlayXP();

            // 2. İspiyoncu: Çarpan şey Player mı?
            Debug.Log("EVET! Player çarptı, XP veriliyor.");

            GameManager.instance.GainXP(xpAmount);
            gameObject.SetActive(false);
        }
    }
}