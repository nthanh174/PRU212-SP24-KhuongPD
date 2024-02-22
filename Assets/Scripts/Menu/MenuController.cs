using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private Button btnStart;
    [SerializeField]
    private Button btnExit;
    [SerializeField]
    private GameObject screenMenu;
    [SerializeField]
    private GameObject screenLevel;
    [SerializeField]
    private GameObject map1;

    [SerializeField]
    private GameObject screenDeath;
    [SerializeField]
    private Button btnHome;
    [SerializeField]
    private Button btnBack;

    // Start is called before the first frame update
    public void Start()
    {
        HideScreen(screenDeath);
        HideScreen(map1);
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
        DisplayScreen(map1);
    }

    public void ClickExit()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
