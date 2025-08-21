using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // Gán Canvas Pause Menu trong Inspector
    [SerializeField] private Button pauseButton; // Gán nút Pause trong Inspector
    [SerializeField] private Button resumeButton; // Gán nút Resume trong Inspector
    [SerializeField] private Button returnToMenuButton; // Gán nút Return to Menu trong Inspector
    private bool isPaused = false;

    void Start()
    {
        // Ẩn menu tạm dừng khi bắt đầu game
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Gán sự kiện cho các nút
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMenu);
    }

    void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Dừng thời gian game
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true); // Hiển thị UI Pause
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(false); // Ẩn nút Pause khi menu hiển thị
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Tiếp tục thời gian game
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false); // Ẩn UI Pause
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(true); // Hiển thị lại nút Pause
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f; // Đặt lại thời gian trước khi chuyển scene
        SceneManager.LoadScene("MainMenu"); // Thay "MainMenu" bằng tên scene menu chính của bạn
    }
}