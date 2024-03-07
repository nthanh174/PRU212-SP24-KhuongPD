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

        // Thêm sự kiện onClick bằng cách gọi phương thức AddListener và truyền vào phương thức RestartGame
        restartButton.onClick.AddListener(RestartGame);
        homeButton.onClick.AddListener(LoadHomeScene);
    }

    public void GameOver()
    {
        // Dừng tất cả hoạt động trong game
        Time.timeScale = 0f;

        // Hiện panel game over
        screenGameOver.SetActive(true);
    }

    void LoadHomeScene()
    {
        // Load lại scene Home bằng SceneManager
        SceneManager.LoadScene("Home");
    }

    public void RestartGame()
    {
        // Đặt lại trạng thái game
        Time.timeScale = 1f;
        screenGameOver.SetActive(false); // Ẩn bảng GameOver

        // Hồi sinh người chơi tại điểm kiểm tra
        playerController.ReturnToCheckPoint();
    }
}
