using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Base Damage Settings")]
    public int damageAmount = 1;

    [Header("Damage Upgrade Settings")]
    public float baseMultiplier = 1f;
    public float multiplierStep = 0.2f;
    public int maxDamageLevel = 5;

    private int damageLevel = 0;
    private float damageMultiplier;

    private void Awake()
    {
        damageMultiplier = baseMultiplier;
    }

    private void Start()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnLevelUp += HandleLevelUp;
        }
    }

    private void OnDestroy()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnLevelUp -= HandleLevelUp;
        }
    }

    private void HandleLevelUp(int newLevel)
    {
        ApplyLevelUpBonus();
    }

    public void TryDealDamage(Collider2D target)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            int finalDamage = Mathf.RoundToInt(damageAmount * damageMultiplier);
            damageable.TakeDamage(finalDamage);

            // Optional: apply knockback
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (target.transform.position - transform.position).normalized;
                rb.AddForce(direction * 5f, ForceMode2D.Impulse);
            }
        }
    }

    public void ApplyLevelUpBonus()
    {
        IncreaseDamageLevel();
    }

    public void IncreaseDamageLevel()
    {
        if (damageLevel < maxDamageLevel)
        {
            damageLevel++;
            damageMultiplier = baseMultiplier + damageLevel * multiplierStep;
            Debug.Log($"Damage level increased to {damageLevel}. New multiplier: {damageMultiplier:0.00}");
        }
    }
}
