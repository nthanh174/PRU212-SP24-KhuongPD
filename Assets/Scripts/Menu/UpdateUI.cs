using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    public TextMeshProUGUI txtHealth;
    public TextMeshProUGUI txtDamage;
    public Button btnDamageUp;
    public Button btnDamageDown;
    public Button btnHealthUp;
    public Button btnHealthDown;
    public Button btnExist;

    public GameObject updateUI;
    public PlayerController playerController;
    public BarUI bar;


    private void Start()
    {

        txtHealth.SetText(playerController.maxHealth.ToString());
        txtDamage.SetText(playerController.maxDamage.ToString());

        btnDamageUp = GameObject.Find("btn+Damage").GetComponent<Button>();
        btnDamageDown = GameObject.Find("btn-Damage").GetComponent<Button>();

        btnHealthUp = GameObject.Find("btn+Health").GetComponent<Button>();
        btnHealthDown = GameObject.Find("btn-Health").GetComponent<Button>();

        btnExist = GameObject.Find("btn-Exits").GetComponent<Button>();

        btnDamageUp.onClick.AddListener(() => UpdateDamage("Up"));
        btnDamageDown.onClick.AddListener(() => UpdateDamage("Down"));

        btnHealthUp.onClick.AddListener(() => UpdateHealth("Up"));
        btnHealthDown.onClick.AddListener(() => UpdateHealth("Down"));

        btnExist.onClick.AddListener(Exits);
    }
    public void UpdateHealth(string action)
    {
        if (action.Equals("Up"))
        {
            if(bar.currentCoin < 10)
            {
                Debug.Log("Bạn không đủ coin");
            }
            else
            {
                playerController.maxHealth += 50;
                bar.UpdateCoinhBar(-10);
                playerController.curentHealth = playerController.maxHealth;
                bar.UpdateHealthBar(playerController.maxHealth, playerController.maxHealth);
            }
        }
        else if (action.Equals("Down"))
        {
            if (playerController.maxHealth > playerController.defaulHealth)
            {
                playerController.maxHealth -= 50;
                playerController.curentHealth = playerController.maxHealth;
                bar.UpdateCoinhBar(10);
                bar.UpdateHealthBar(playerController.maxHealth, playerController.maxHealth);
            }
            else
            {
                Debug.Log("Máu là về mức mặc định");
            }
        }
        txtHealth.SetText(playerController.maxHealth.ToString());
    }

    public void UpdateDamage(string action)
    {
        if (action.Equals("Up"))
        {
            if (bar.currentCoin < 10)
            {
                Debug.Log("Bạn không đủ coin");
            }
            else
            {
                playerController.maxDamage += 10;
                bar.UpdateCoinhBar(-10);
            }
        }
        else if (action.Equals("Down"))
        {
            if (playerController.maxDamage > playerController.defaulDamage)
            {
                playerController.maxDamage -= 10;
                bar.UpdateCoinhBar(10);
            }
            else
            {
                Debug.Log("Damage là về mức mặc định");
            }
        }
        txtDamage.SetText(playerController.maxDamage.ToString());
    }

    public void Confirm()
    {
        Time.timeScale = 1f;
        isActive(false);
    }
    public void Exits()
    {
        Time.timeScale = 1f;
        isActive(false);
    }

    public void isActive(bool active)
    {
        updateUI.SetActive(active);
    }
}
