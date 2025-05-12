using UnityEngine;
public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 1;
    public float damageMultiplier = 1f;

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
            damageable.TakeDamage(Mathf.RoundToInt(damageAmount * damageMultiplier));

            // Optional: apply knockback if needed
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
        Debug.Log("Applying level up bonus to damage dealer.");
        damageMultiplier += 0.1f;
    }
}
