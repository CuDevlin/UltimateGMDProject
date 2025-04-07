using UnityEngine;
public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 1;

    public void TryDealDamage(Collider2D target)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);

            // Optional: apply knockback if needed
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (target.transform.position - transform.position).normalized;
                rb.AddForce(direction * 5f, ForceMode2D.Impulse);
            }
        }
    }
}
