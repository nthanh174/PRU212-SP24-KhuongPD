using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    public Image redBar;
    public TextMeshProUGUI txtHealth;
    public TextMeshProUGUI txtCoin;

    private int currentCoin = 0;
    private int getCoin = 0;
    private int startCoin = 0;

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        redBar.fillAmount = (float)currentHealth / (float)maxHealth;
        txtHealth.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    public void UpdateCoinhBar(int coin)
    {
        getCoin += coin;
        currentCoin = startCoin + getCoin;
        txtCoin.text = currentCoin.ToString();
    }
}
