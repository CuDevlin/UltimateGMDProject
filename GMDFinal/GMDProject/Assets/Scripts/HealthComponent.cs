using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int baseMaxHealth = 5;
    public int healthUpgradeStep = 10;
    public int maxHealthLevel = 10;
    public int healthUpgradeOnLevelUp = 1;

    private int healthLevel = 0;
    private int levelBonusHealth = 0;

    public int maxHealth;
    private int currentHealth;

    public bool isPlayer = false;
    public UnityEvent<int, int> OnHealthChanged;

    private void Awake()
    {
        RecalculateMaxHealth();
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
            ExperienceManager.Instance.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        if (isPlayer && ExperienceManager.Instance != null)
            ExperienceManager.Instance.OnLevelUp -= HandleLevelUp;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
            UIHandler.instance?.SetHealthText(currentHealth, maxHealth);
            SoundManager.Instance?.PlaySFX(SoundManager.Instance.hitPlayerClip);
        }
        else
        {
            SoundManager.Instance?.PlaySFX(SoundManager.Instance.hitEnemyClip);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (TryGetComponent(out EnemyController enemyController))
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
        levelBonusHealth += healthUpgradeOnLevelUp;
        RecalculateMaxHealth();
        currentHealth = maxHealth;

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(1f);
            UIHandler.instance?.SetHealthText(currentHealth, maxHealth);
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"[LEVEL-UP] Bonus health increased by {healthUpgradeOnLevelUp}, total bonus: {levelBonusHealth}, maxHealth now: {maxHealth}");
    }

    public void IncreaseHealthLevel()
    {
        if (healthLevel >= maxHealthLevel)
        {
            Debug.Log("[POWERUP] Max health level reached.");
            return;
        }

        healthLevel++;
        RecalculateMaxHealth();
        currentHealth = maxHealth;

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(1f);
            UIHandler.instance?.SetHealthText(currentHealth, maxHealth);
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"[POWERUP] Health level increased to {healthLevel}. Max health now: {maxHealth}");
    }

    private void RecalculateMaxHealth()
    {
        maxHealth = baseMaxHealth + (healthLevel * healthUpgradeStep) + levelBonusHealth;
    }
}
