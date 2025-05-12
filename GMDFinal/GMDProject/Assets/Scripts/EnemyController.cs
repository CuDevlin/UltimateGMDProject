using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float speed = 2f;
    public int experienceReward = 10;

    private Rigidbody2D rb;
    private Transform player;
    private bool isChasing = true;

    public event Action<GameObject> OnEnemyDestroyed;

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

    public void DisableMovement()
    {
        isChasing = false;
        rb.simulated = false;
    }

    public void HandleEnemyDeath()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.GainExperience(experienceReward);
        }
          else
        {
            Debug.LogWarning("ExperienceManager instance is null.");
        }

        OnEnemyDestroyed?.Invoke(gameObject);
    }

    private void OnDestroy()
    {
    }
}
