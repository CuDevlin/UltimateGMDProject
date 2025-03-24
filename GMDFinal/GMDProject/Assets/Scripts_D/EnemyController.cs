using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Public variables
    public float speed;
    public float changeTime = 3.0f;
    
    // Private variables
    private Rigidbody2D rigidbody2d;
    private float timer;
    private bool broken = true;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
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
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    public void Fix()
    {
        broken = false;
        GetComponent<Rigidbody2D>().simulated = false;
    }
}