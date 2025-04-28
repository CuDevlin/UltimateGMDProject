using UnityEngine;
public class EnemyController : MonoBehaviour
{
    public float speed = 2f;

    private Rigidbody2D rb;
    private Transform player;
    private bool isChasing = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        if (!isChasing || player == null) return;

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        Vector2 newPosition = rb.position + direction * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    // Optional: Hook up to HealthComponent death event
    public void DisableMovement()
    {
        isChasing = false;
        rb.simulated = false;
    }
}