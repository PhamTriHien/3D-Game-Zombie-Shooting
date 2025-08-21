using UnityEngine;
using UnityEngine.SceneManagement; // Để chuyển cảnh
using UnityEngine.UI; // Để sử dụng Button

public class MainMenu : MonoBehaviour
{
    // Khai báo các nút trong Inspector
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        // Gắn sự kiện cho nút Play
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
        }
        else
        {
            Debug.LogWarning("Play Button chưa được gán trong Inspector!");
        }

        // Gắn sự kiện cho nút Exit
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogWarning("Exit Button chưa được gán trong Inspector!");
        }
    }

    // Hàm xử lý khi nhấn nút Play
    public void PlayGame()
    {
        // Chuyển sang cảnh game (thay "GameScene" bằng tên cảnh bạn muốn chuyển tới)
        SceneManager.LoadScene("ShootingRange");
        // Nếu bạn muốn chuyển cảnh theo index, dùng: SceneManager.LoadScene(1);
    }

    // Hàm xử lý khi nhấn nút Exit
    public void ExitGame()
    {
        // Thoát game
        Debug.Log("Thoát game!"); // Hiển thị thông báo trong Editor
        Application.Quit(); // Thoát ứng dụng khi chạy ở build
    }
}