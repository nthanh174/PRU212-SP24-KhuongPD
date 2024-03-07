using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public Button btnStart;
    public Button btnExit;
    public GameObject screenMenu;
    public GameObject map1;
    public GameObject player;
    public GameObject screenBar;
    public GameObject screenDeath;
    public Button btnHome;
    public Button btnBack;

    // Start is called before the first frame update
    public void Start()
    {
        HideScreen(screenDeath);
        HideScreen(map1);
        HideScreen(player);
        HideScreen(screenBar);
        HideScreen(screenDeath);
        DisplayScreen(screenMenu);
        AddListener();
    }

    // Update is called once per frame
    public void Update()
    {

    }
    //--------------------------------------------------------------------------------------------------

    public void AddListener()
    {
        //btnMapLV1.onClick.AddListener();
        btnStart.onClick.AddListener(ClickStart);
        btnExit.onClick.AddListener(ClickExit);
        btnHome.onClick.AddListener(ClickBtnHome);
        btnBack.onClick.AddListener(ClickBtnBack);
    }
    public void HideScreen(GameObject go)
    {
        if (go != null)
        {
            go.SetActive(false);
        }
    }
    public void DisplayScreen(GameObject go)
    {
        if (go != null)
        {
            go.SetActive(true);
        }
    }

    public void ClickBtnBack()
    {
        HideScreen(screenDeath);
        DisplayScreen(map1);

    }
    public void ClickBtnHome()
    {
        HideScreen(screenDeath);

        // Hiển thị trang home
        DisplayScreen(screenMenu);
    }
    public void ClickStart()
    {
        HideScreen(screenMenu);
        DisplayScreen(player);
        DisplayScreen(screenBar);
        DisplayScreen(map1);
    }

    public void ClickExit()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
