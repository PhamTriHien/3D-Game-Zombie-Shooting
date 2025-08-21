using UnityEngine;
using UnityEngine.UI;

public class GunRaycasting : MonoBehaviour
{
    [SerializeField] private Transform _firingPos;
    [SerializeField] private Transform _aimingCamera;
    [SerializeField] private Transform _hitMarkerPrefab;
    [SerializeField] private float _baseDamage = 20f; // Sát thương cơ bản của súng
    [SerializeField] private Text _ammoText; // Text UI để hiển thị số đạn
    [SerializeField] private Text _scoreText; // Text UI để hiển thị điểm số
    [SerializeField] private Text _leverText; // Text UI để hiển thị cấp độ
    [SerializeField] private float _normalFOV = 60f; // FOV bình thường của camera
    [SerializeField] private float _aimFOV = 40f; // FOV khi ngắm
    [SerializeField] private float _aimSpeed = 5f; // Tốc độ chuyển đổi FOV khi ngắm

    private int _maxAmmo = 24; // Số đạn tối đa trong băng
    private int _currentAmmo; // Số đạn hiện tại trong băng
    private Camera _camera; // Tham chiếu đến Camera component
    private bool _isMouseLocked = false; // Trạng thái khóa tâm chuột
    private int _score = 0; // Điểm số của người chơi
    private int _lever = 1; // Cấp độ của người chơi
    private const int SCORE_PER_ZOMBIE = 1; // Điểm cho mỗi zombie bị giết
    private const int SCORE_PER_LEVER = 5; // Điểm cần để lên cấp
    private const float DAMAGE_INCREASE_PER_LEVER = 0.04f; // Tăng 4% sát thương mỗi cấp

    private void Start()
    {
        _currentAmmo = _maxAmmo; // Khởi tạo băng đạn đầy
        _camera = _aimingCamera.GetComponent<Camera>(); // Lấy component Camera
        UpdateAmmoUI(); // Cập nhật giao diện đạn
        UpdateScoreUI(); // Cập nhật giao diện điểm số
        UpdateLeverUI(); // Cập nhật giao diện cấp độ
    }

    private void Update()
    {
        // Kiểm tra phím R để nạp đạn
        if (Input.GetKeyDown(KeyCode.R) && _currentAmmo < _maxAmmo)
        {
            Reload();
        }

        // Xử lý ngắm khi giữ CTRL trái
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _aimFOV, Time.deltaTime * _aimSpeed);
        }
        else
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _normalFOV, Time.deltaTime * _aimSpeed);
        }

        // Xử lý khóa/mở khóa tâm chuột khi nhấn Caps Lock
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            _isMouseLocked = !_isMouseLocked; // Chuyển đổi trạng thái khóa
            if (_isMouseLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("Tâm chuột đã khóa");
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Tâm chuột đã mở khóa");
            }
        }
    }

    public void PerformRaycast()
    {
        // Kiểm tra xem còn đạn để bắn không
        if (_currentAmmo <= 0)
        {
            Debug.Log("Hết đạn! Nhấn R để nạp đạn.");
            return;
        }

        Ray aimingRay = new(_aimingCamera.position, _aimingCamera.forward);
        if (Physics.Raycast(aimingRay, out var raycastHit))
        {
            Instantiate(_hitMarkerPrefab, raycastHit.point, Quaternion.LookRotation(raycastHit.normal), parent: raycastHit.collider.transform);
            if (raycastHit.collider.TryGetComponent<ZombieAnimationController>(out var zombie))
            {
                float damage = CalculateDamage(); // Tính sát thương dựa trên cấp độ
                zombie.TakeDamage(damage); // Gây sát thương cho zombie
            }
        }

        _currentAmmo--; // Giảm số đạn sau mỗi lần bắn
        UpdateAmmoUI(); // Cập nhật giao diện đạn
    }

    // Tính sát thương dựa trên cấp độ
    private float CalculateDamage()
    {
        return _baseDamage * (1f + (_lever - 1) * DAMAGE_INCREASE_PER_LEVER);
    }

    // Gọi khi zombie bị giết để tăng điểm
    public void AddScore()
    {
        _score += SCORE_PER_ZOMBIE;
        UpdateScoreUI();
        CheckLeverUp();
    }

    private void CheckLeverUp()
    {
        int requiredScore = _lever * SCORE_PER_LEVER;
        if (_score >= requiredScore)
        {
            _lever++;
            Debug.Log($"Lên cấp {_lever}! Sát thương tăng {(DAMAGE_INCREASE_PER_LEVER * 100)}%.");
            UpdateLeverUI();
        }
    }

    private void Reload()
    {
        _currentAmmo = _maxAmmo;
        UpdateAmmoUI();
        Debug.Log("Đã nạp đạn!");
    }

    private void UpdateAmmoUI()
    {
        if (_ammoText != null)
        {
            _ammoText.text = $"{_currentAmmo}/∞";
        }
    }

    private void UpdateScoreUI()
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {_score}";
        }
    }

    private void UpdateLeverUI()
    {
        if (_leverText != null)
        {
            _leverText.text = $"Lever: {_lever}";
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}