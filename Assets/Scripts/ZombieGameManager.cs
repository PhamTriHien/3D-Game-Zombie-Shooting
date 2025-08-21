using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ZombieGameManager : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab; // Prefab của zombie
    [SerializeField] private Transform[] spawnPoints; // 3 vị trí spawn
    [SerializeField] private GameObject winGameUI; // UI hiển thị khi thắng game
    [SerializeField] private Text roundText; // Text hiển thị số round hiện tại
    [SerializeField] private float roundDelay = 3f; // Thời gian chờ giữa các round

    private int currentRound = 0;
    private List<GameObject> activeZombies = new List<GameObject>();
    private bool isGameOver = false;

    // Cấu hình số lượng zombie mỗi round
    private readonly int[] zombiesPerPointPerRound = { 2, 4, 5 }; // Số zombie mỗi điểm spawn ở các round
    private readonly int[] totalZombiesPerRound = { 6, 12, 15 }; // Tổng số zombie mỗi round

    void Start()
    {
        // Kiểm tra các giá trị cần thiết
        if (zombiePrefab == null || !zombiePrefab.GetComponent<ZombieAnimationController>())
        {
            Debug.LogError("Zombie Prefab không được gán hoặc thiếu ZombieAnimationController!");
            return;
        }
        if (spawnPoints.Length != 3)
        {
            Debug.LogError("Cần đúng 3 điểm spawn!");
            return;
        }
        if (winGameUI == null)
        {
            Debug.LogError("WinGame UI không được gán!");
            return;
        }
        if (roundText == null)
        {
            Debug.LogError("Round Text không được gán!");
            return;
        }

        winGameUI.SetActive(false);
        StartNextRound();
    }

    void Update()
    {
        if (isGameOver) return;

        // Kiểm tra nếu tất cả zombie trong round hiện tại đã bị tiêu diệt
        if (activeZombies.Count > 0 && activeZombies.TrueForAll(z => z == null))
        {
            activeZombies.Clear();
            if (currentRound < 3)
            {
                Invoke(nameof(StartNextRound), roundDelay);
            }
            else
            {
                EndGame();
            }
        }
    }

    void StartNextRound()
    {
        currentRound++;
        if (currentRound > 3) return;

        // Cập nhật UI round
        roundText.text = $"Round: {currentRound}";

        // Spawn zombie theo round
        int zombiesPerPoint = zombiesPerPointPerRound[currentRound - 1];
        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < zombiesPerPoint; i++)
            {
                // Tạo vị trí spawn ngẫu nhiên gần spawnPoint
                Vector3 spawnPos = spawnPoint.position + Random.insideUnitSphere * 2f;
                spawnPos.y = spawnPoint.position.y; // Giữ zombie ở mặt đất
                GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
                activeZombies.Add(zombie);

                // Tăng chỉ số cho zombie ở round 3
                if (currentRound == 3)
                {
                    ZombieAnimationController zombieController = zombie.GetComponent<ZombieAnimationController>();
                    if (zombieController != null)
                    {
                        // Tăng 5% HP
                        var maxHealthField = zombieController.GetType().GetField("_maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (maxHealthField != null)
                        {
                            float? currentMaxHealth = maxHealthField.GetValue(zombieController) as float?;
                            if (currentMaxHealth.HasValue)
                            {
                                maxHealthField.SetValue(zombieController, currentMaxHealth.Value * 1.05f);
                            }
                        }

                        // Tăng 5% speed
                        var runSpeedField = zombieController.GetType().GetField("runSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (runSpeedField != null)
                        {
                            float? currentRunSpeed = runSpeedField.GetValue(zombieController) as float?;
                            if (currentRunSpeed.HasValue)
                            {
                                runSpeedField.SetValue(zombieController, currentRunSpeed.Value * 1.05f);
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"Bắt đầu Round {currentRound}: Spawn {totalZombiesPerRound[currentRound - 1]} zombies.");
    }

    void EndGame()
    {
        isGameOver = true;
        winGameUI.SetActive(true);
        Time.timeScale = 0f; // Dừng game
        Debug.Log("Chúc mừng! Bạn đã tiêu diệt hết zombie và thắng game!");
    }
}