using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZombieAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string isWalkingParam = "IsWalking";
    [SerializeField] private string isRunningParam = "IsRunning";
    [SerializeField] private string isAttackingParam = "IsAttacking";
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float attackDuration = 1.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float runRange = 2.5f;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private float _maxHealth = 50f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackInterval = 0.5f;
    private float _currentHealth;
    private Transform player;
    private bool isDead = false;
    private bool isAttacking = false;
    private float currentSpeed = 0f;
    private float lastAttackTime = 0f;
    private Rigidbody rb;
    private GunRaycasting _gun; // Tham chiếu đến GunRaycasting để gọi AddScore

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError($"Không tìm thấy GameObject với tag 'Player' cho {gameObject.name}");
            return;
        }
        if (!player.TryGetComponent<PlayerMovement>(out _))
        {
            Debug.LogError($"GameObject 'Player' không có component PlayerMovement!");
        }
        _gun = player.GetComponent<GunRaycasting>(); // Lấy tham chiếu đến GunRaycasting
        if (_gun == null)
        {
            Debug.LogError($"GameObject 'Player' không có component GunRaycasting!");
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"Thiếu thành phần Animator trên {gameObject.name}");
                enabled = false;
                return;
            }
        }
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        if (runRange < attackRange)
        {
            Debug.LogWarning($"runRange ({runRange}) nên lớn hơn hoặc bằng attackRange ({attackRange}) để tránh xung đột logic.");
            runRange = attackRange;
        }
        _currentHealth = _maxHealth;
        if (_healthBar != null)
        {
            _healthBar.maxValue = _maxHealth;
            _healthBar.value = _currentHealth;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;
        UpdateAnimationAndMovement();
    }

    void UpdateAnimationAndMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isRunning = false;

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            StartAttack();
        }
        else if (distanceToPlayer > attackRange && isAttacking)
        {
            EndAttack();
        }

        if (!isAttacking)
        {
            if (distanceToPlayer > runRange)
            {
                isRunning = true;
                currentSpeed = runSpeed;
            }
            else
            {
                isRunning = false;
                currentSpeed = 0f;
            }
        }
        else
        {
            currentSpeed = 0f;
            if (Time.time - lastAttackTime >= attackInterval && distanceToPlayer <= attackRange)
            {
                DealDamageToPlayer();
                lastAttackTime = Time.time;
            }
        }

        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool(isWalkingParam, false);
        animator.SetBool(isRunningParam, isRunning);

        if (!isAttacking && distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }

        Debug.Log($"Distance: {distanceToPlayer:F2}, AttackRange: {attackRange:F2}, RunRange: {runRange:F2}, IsAttacking: {isAttacking}, IsRunning: {isRunning}, Speed: {currentSpeed:F2}");
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        float speed = currentSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed);
    }

    void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger(isAttackingParam);
            lastAttackTime = Time.time - attackInterval;
            StartCoroutine(AttackCoroutine());
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        animator.ResetTrigger(isAttackingParam);
    }

    IEnumerator AttackCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                EndAttack();
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        EndAttack();
    }

    void DealDamageToPlayer()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (player.TryGetComponent<PlayerMovement>(out var playerMovement))
            {
                playerMovement.TakeDamage(attackDamage);
                Debug.Log($"Zombie gây {attackDamage} sát thương cho người chơi!");
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0) return;
        Debug.Log($"Zombie nhận {damage} sát thương. Máu hiện tại: {_currentHealth} -> {Mathf.Max(_currentHealth - damage, 0)}");
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

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log($"Zombie {gameObject.name} đã chết, ngã xuống đất.");

        // Thông báo cho GunRaycasting để tăng điểm
        if (_gun != null)
        {
            _gun.AddScore();
        }

        if (animator != null)
        {
            animator.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.up * 2f + Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }

        Destroy(gameObject, 2f);
    }
}