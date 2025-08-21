using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Thêm để quản lý Scene
using TMPro; // Thêm để sử dụng TextMeshPro nếu cần

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Slider _healthBar; // Thanh máu UI
    [SerializeField] private float _maxHealth = 100f; // Máu tối đa
    [SerializeField] private GameObject _gameOverCanvas; // Canvas Game Over
    private float _currentHealth; // Máu hiện tại
    private float _velocityY;

    private void Awake()
    {
        _currentHealth = _maxHealth; // Khởi tạo máu
        if (_healthBar != null)
        {
            _healthBar.maxValue = _maxHealth;
            _healthBar.value = _currentHealth;
        }
        if (_gameOverCanvas != null)
        {
            _gameOverCanvas.SetActive(false); // Ẩn UI Game Over khi bắt đầu
        }
    }

    private void Update()
    {
        if (_currentHealth <= 0) return; // Không di chuyển khi đã chết
        var input = _moveAction.action.ReadValue<Vector2>();
        var direction = transform.forward * input.y + transform.right * input.x;
        var newVelocity = direction * _moveSpeed;
        if (_jumpAction.action.triggered && _controller.isGrounded)
        {
            _velocityY = _jumpForce;
        }
        else
        {
            UpdateFalling();
        }
        newVelocity.y = _velocityY;
        _controller.Move(newVelocity * Time.deltaTime);
    }

    private void UpdateFalling()
    {
        if (_controller.isGrounded)
        {
            _velocityY = -1;
        }
        else
        {
            _velocityY += Physics.gravity.y * Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0) return; // Đã chết, không nhận thêm sát thương
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        UpdateHealthBar();
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (_healthBar != null)
        {
            _healthBar.value = _currentHealth;
        }
    }

    private void Die()
    {
        // Xử lý khi người chơi chết
        _controller.enabled = false; // Vô hiệu hóa di chuyển
        Time.timeScale = 0f; // Tạm dừng game
        if (_gameOverCanvas != null)
        {
            _gameOverCanvas.SetActive(true); // Hiển thị UI Game Over
        }
        Debug.Log("Người chơi đã chết!");
    }

    // Hàm cho nút Restart Game
    public void RestartGame()
    {
        Time.timeScale = 1f; // Khôi phục thời gian
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Tải lại Scene hiện tại
    }

    // Hàm cho nút Return to Menu
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Khôi phục thời gian
        SceneManager.LoadScene("MainMenu"); // Tải Scene Menu (giả sử Scene Menu có index 0)
    }
}