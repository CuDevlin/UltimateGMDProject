using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 6f;
    public float maxLifetime = 5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Automatically destroy after some time
        Destroy(gameObject, maxLifetime);
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;

        // Rotate to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Let DamageDealer handle the actual damage logic
        DamageDealer damageDealer = GetComponent<DamageDealer>();
        if (damageDealer != null)
        {
            damageDealer.TryDealDamage(other);
        }

        // Then destroy self
        Destroy(gameObject);
    }
}