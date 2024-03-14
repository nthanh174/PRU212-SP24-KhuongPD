using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public PlayerController playerController;

    public GameObject screenGameOver;
    public Button restartButton;
    public Button homeButton;

    void Start()
    {
        restartButton = GameObject.Find("btnRestart").GetComponent<Button>();
        homeButton = GameObject.Find("btnHome").GetComponent<Button>();

        restartButton.onClick.AddListener(RestartGame);
        homeButton.onClick.AddListener(LoadHomeScene);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        screenGameOver.SetActive(true);
    }

    void LoadHomeScene()
    {
        SceneManager.LoadScene("Home");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        screenGameOver.SetActive(false);
        playerController.ReturnToCheckPoint();
    }
}
