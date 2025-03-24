using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;

    public int damage = 1;

    // Awake is called when the Projectile GameObject is instantiated
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Destroy the projectile if it moves too far (out of bounds or after a certain distance)
        if (transform.position.magnitude > 25.0f)
        {
            Destroy(gameObject);
        }
    }

    // Launch the projectile in a certain direction with a given speed
    public void Launch(Vector2 direction, float speed)
    {
        // Set the velocity directly (this makes it move immediately in the desired direction)
        rigidbody2d.linearVelocity = direction.normalized * speed;

        // Rotate the projectile to face the direction it is moving
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Convert the direction to an angle
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Apply the rotation to the projectile
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            // Debug log to check if the projectile hit the enemy
            Debug.Log("Projectile hit enemy!");

            // Apply knockback effect
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            enemy.ApplyKnockback(knockbackDirection, 5f); // Knockback force

            // Deal damage to the enemy
            enemy.TakeDamage(damage); // Reduce enemy health by 1 (adjust as needed)

            // Destroy the projectile
            Destroy(gameObject);
        }
    }
}