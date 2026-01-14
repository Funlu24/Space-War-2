using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    // Ortak işi yapan yardımcı fonksiyon
    void ApplyUpgradeToPlayer(int type)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerConroller player = playerObj.GetComponent<PlayerConroller>();
            if (player != null)
            {
                if (type == 1) player.UpgradeSpeed();
                else if (type == 2) player.UpgradeFireRate();
                else if (type == 3) player.UpgradeHealth();
            }
        }

        // İşi bitince paneli kapat
        GameManager.instance.CloseLevelUpPanel();
    }

    // --- BUTONLARIN ÇAĞIRACAĞI GARANTİ FONKSİYONLAR ---
    // Unity Inspector'da bunlar kesinlikle görünür.

    public void Button_Hiz()
    {
        ApplyUpgradeToPlayer(1);
    }

    public void Button_AtesHizi()
    {
        ApplyUpgradeToPlayer(2);
    }

    public void Button_Can()
    {
        ApplyUpgradeToPlayer(3);
    }
}