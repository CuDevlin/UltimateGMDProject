using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.up;
    private Rigidbody2D rb;
    private ProjectileShooter shooter;

    private bool controlsEnabled = false; // New flag to control player input

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        shooter = GetComponent<ProjectileShooter>();
    }

    public void EnableControls(bool enable)
    {
        controlsEnabled = enable;

        if (!enable)
        {
            moveInput = Vector2.zero; // Stop movement when disabled
        }
    }

    public void OnMove(InputValue value)
    {
        if (!controlsEnabled) return;

        moveInput = value.Get<Vector2>();

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput.normalized;
        }
    }

    public void OnFire()
    {
        if (!controlsEnabled) return;

        if (shooter != null)
        {
            Vector2 shootDirection = moveInput != Vector2.zero ? moveInput.normalized : lastMoveDirection;
            shooter.TryFire(shootDirection);
        }
    }

    void FixedUpdate()
    {
        if (!controlsEnabled) return;

        Vector2 newPosition = rb.position + moveInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}