using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 5;
    private int currentHealth;
    public bool isPlayer = false;

    public UnityEvent<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
        }
    }

    private void OnEnable()
    {
        if (isPlayer && ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnLevelUp += HandleLevelUp;
        }
    }

    private void OnDisable()
    {
        if (isPlayer && ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnLevelUp -= HandleLevelUp;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.DisableMovement();
            enemyController.HandleEnemyDeath();  // Explicitly call it before destruction
        }

        if (isPlayer)
        {
            Debug.Log("Player died");
        }

        Destroy(gameObject);
    }

    private void HandleLevelUp(int newLevel)
    {
        ApplyLevelUpBonus(newLevel);
    }

    private void ApplyLevelUpBonus(int level)
    {
        int healthBonus = Mathf.FloorToInt(level * 1.2f);  // Example: Bonus increases with level
        maxHealth += healthBonus;
        currentHealth = maxHealth;

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
        }

        Debug.Log($"Level Up! New Max Health: {maxHealth}");
    }
}
