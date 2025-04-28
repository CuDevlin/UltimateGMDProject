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
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        if (isPlayer)
        {
            UIHandler.instance?.SetHealthValue(currentHealth / (float)maxHealth);
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
}