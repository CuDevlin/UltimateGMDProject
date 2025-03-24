using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Public variables
    public float speed;

    // Public variables for health
    public int maxHealth = 2;
    private int currentHealth;
    public int health { get { return currentHealth; }}
    
    // Private variables
    private Rigidbody2D rigidbody2d;
    private bool broken = true;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth; // Initialize health
    }

    // Update is called every frame
    void Update()
    {
        if (player == null)
        {
            return;
        }
    }

    // FixedUpdate has the same call rate as the physics system
    void FixedUpdate()
    {
        if (!broken || player == null)
        {
            return;
        }
        
        Vector2 position = rigidbody2d.position;
        Vector2 direction = ((Vector2)player.position - position).normalized;
        position += direction * speed * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collision is with a player
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1); // Player is hit
        }

        // Check if the collision is with a projectile
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            // Apply knockback
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            ApplyKnockback(knockbackDirection, 5f); // Knockback force
        }
    }

    public void Fix()
    {
        broken = false;
        GetComponent<Rigidbody2D>().simulated = false;
    }

    public void TakeDamage(int damage)
    {
        // Decrease health when taking damage
        currentHealth -= damage;
        // Ensure health doesn't go below zero
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // If health reaches zero, handle enemy death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle enemy death (destroy or disable)
        Destroy(gameObject); // Only destroy when health is zero
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        // Check if the enemy has a Rigidbody2D
        Rigidbody2D enemyRigidbody = GetComponent<Rigidbody2D>();
        if (enemyRigidbody != null)
        {
            // Apply an impulse force to the enemy's Rigidbody2D
            enemyRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}
