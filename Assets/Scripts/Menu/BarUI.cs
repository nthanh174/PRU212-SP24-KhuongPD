using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    public Image redBar;
    public TextMeshProUGUI txtHealth;
    public TextMeshProUGUI txtCoin;

    public int currentCoin = 0;

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        redBar.fillAmount = (float)currentHealth / (float)maxHealth;
        txtHealth.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    public void UpdateCoinhBar(int coin)
    {
        currentCoin += coin;
        txtCoin.text = currentCoin.ToString();
    }
}
