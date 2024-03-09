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
    public Button btnConfirm;
    public Button btnExist;

    public GameObject updateUI;
    public PlayerController playerController;

    BarUI bar;

    private void Start()
    {
        btnDamageUp = GameObject.Find("btn+Damage").GetComponent<Button>();
        btnDamageDown = GameObject.Find("btn-Damage").GetComponent<Button>();

        btnHealthUp = GameObject.Find("btn+Health").GetComponent<Button>();
        btnHealthDown = GameObject.Find("btn-Health").GetComponent<Button>();

        btnConfirm = GameObject.Find("btn-Confirm").GetComponent<Button>();
        btnExist = GameObject.Find("btn-Exits").GetComponent<Button>();

        // Thêm sự kiện onClick bằng cách gọi phương thức AddListener và truyền vào phương thức RestartGame
        btnDamageUp.onClick.AddListener(UpdateDamage);
        btnDamageDown.onClick.AddListener(UpdateDamage);

        btnHealthUp.onClick.AddListener(UpdateHealth);
        btnHealthDown.onClick.AddListener(UpdateHealth);

        btnConfirm.onClick.AddListener(Confirm);
        btnExist.onClick.AddListener(Exits);
    }
    public void UpdateHealth()
    {

    }

    public void UpdateDamage()
    {

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
