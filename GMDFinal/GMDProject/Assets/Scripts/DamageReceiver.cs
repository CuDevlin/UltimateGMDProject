using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [Tooltip("How much damage this object deals on contact")]
    public int contactDamage = 1;

    [Tooltip("Cooldown between applying damage (seconds)")]
    public float damageCooldown = 2f;

    private float lastDamageTime;

    private bool isInsideDamageZone = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isInsideDamageZone = true; // Player has entered the damage zone
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isInsideDamageZone = false; // Player has exited the damage zone
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideDamageZone = true; // Player has entered the damage zone
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideDamageZone = false; // Player has exited the damage zone
        }
    }

    private void Update()
    {
        if (isInsideDamageZone && Time.time >= lastDamageTime + damageCooldown)
        {
            // Apply damage to the player if the cooldown has passed
            ApplyDamageToPlayer();
            lastDamageTime = Time.time; // Reset the damage cooldown timer
        }
    }

    private void ApplyDamageToPlayer()
    {
        // Find the player's HealthComponent and apply damage if it's the correct player
        HealthComponent playerHealth = GameObject.FindWithTag("Player")?.GetComponent<HealthComponent>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(contactDamage); // Deal damage to the player's health
        }
    }
}