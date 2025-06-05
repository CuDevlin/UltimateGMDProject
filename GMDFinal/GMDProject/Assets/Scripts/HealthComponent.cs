using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int baseMaxHealth = 5;
    public int healthUpgradeStep = 10;
    public int maxHealthLevel = 5;

    private int healthLevel = 0;
    public int maxHealth;
    private int currentHealth;

    public bool isPlayer = false;
    public UnityEvent<int, int> OnHealthChanged;

    private void Awake()
    {
        maxHealth = baseMaxHealth + healthLevel * healthUpgradeStep;
        currentHealth = maxHealth;

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
            UIHandler.instance?.SetHealthText(currentHealth, maxHealth);
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
            UIHandler.instance?.SetHealthText(currentHealth, maxHealth);
            SoundManager.Instance.PlaySFX(SoundManager.Instance.hitPlayerClip);
        }

        if (!isPlayer)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.hitEnemyClip);
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
            enemyController.HandleEnemyDeath();
        }

        if (isPlayer)
        {
            Debug.Log("Player died");
            UIHandler.instance?.ShowDeathPopup();
        }

        Destroy(gameObject);
    }

    private void HandleLevelUp(int newLevel)
    {
        ApplyLevelUpBonus(newLevel);
    }

    private void ApplyLevelUpBonus(int level)
    {
        IncreaseHealthLevel();
    }

    public void IncreaseMaxHealth(int flatAmount)
    {
        maxHealth += flatAmount;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
            UIHandler.instance?.SetHealthText(currentHealth, maxHealth);
        }

        Debug.Log($"Max health increased by {flatAmount}. New max: {maxHealth}");
    }

    public void IncreaseHealthLevel()
    {
        if (healthLevel >= maxHealthLevel)
        {
            Debug.Log("[HEALTH] Max health level reached.");
            return;
        }

        healthLevel++;
        maxHealth = baseMaxHealth + healthLevel * healthUpgradeStep;
        currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
            UIHandler.instance?.SetHealthText(currentHealth, maxHealth);
        }

        Debug.Log($"[HEALTH] Level increased to {healthLevel}. Max health is now {maxHealth}.");
    }
}
