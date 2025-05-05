using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour, IDamageable
{
    public int maxHealth = 5;
    private int currentHealth;
    public bool isPlayer = false;

    public UnityEvent<int, int> OnHealthChanged; // Optional if you prefer UnityEvents
    public ExperienceManager experienceManager;

    void Awake()
    {
        currentHealth = maxHealth;

        // Update UI if this is the player
        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth); // Initialize health bar
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth); // Update health bar
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // You can play death animation or do something else
        if (isPlayer)
        {
            Debug.Log("Player died");
        }

        Destroy(gameObject);
        experienceManager.GainExperience(10);
    }

    // You can call this method when the player's health upgrades
    public void UpgradeHealth(int additionalHealth)
    {
        maxHealth += additionalHealth;
        currentHealth = maxHealth; // Set health to max if upgraded
        if (isPlayer)
        {
            UIHandler.instance?.UpdateMaxHealth(maxHealth); // Update the max health bar size
            UIHandler.instance?.SetHealthValue(1.0f); // Update health to full
        }
    }
}