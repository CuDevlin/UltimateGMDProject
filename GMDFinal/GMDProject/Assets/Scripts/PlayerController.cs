using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Base Settings")]
    public float baseSpeed = 3f;
    public float speedUpgradeStep = 0.5f;
    public int maxSpeedLevel = 5;
    private int speedLevel = 0;

    [Header("Animation")]
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 lastMoveDirection = Vector2.up;
    private Vector2 lastLookDirection = Vector2.right;

    private Rigidbody2D rb;
    private ProjectileShooter shooter;
    private InputSystem_Actions controls;

    private bool controlsEnabled = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        shooter = GetComponent<ProjectileShooter>();
        controls = new InputSystem_Actions();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        PowerUpManager powerUpManager = FindFirstObjectByType<PowerUpManager>();
        powerUpManager?.RegisterPlayer(this.gameObject);
    }

    void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;

        controls.Player.Look.performed += OnLook;
        controls.Player.Look.canceled += OnLook;

        controls.Player.Attack.performed += OnFire;
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    public void EnableControls(bool enable)
    {
        controlsEnabled = enable;

        if (!enable)
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (!controlsEnabled) return;

        moveInput = context.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput.normalized;
        }
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        if (!controlsEnabled) return;

        lookInput = context.ReadValue<Vector2>();

        if (lookInput != Vector2.zero)
        {
            lastLookDirection = lookInput.normalized;
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (!controlsEnabled || shooter == null) return;

        Vector2 shootDirection;

        if (lookInput != Vector2.zero)
        {
            lastLookDirection = lookInput.normalized;
            shootDirection = lastLookDirection;
        }
        else if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput.normalized;
            shootDirection = lastMoveDirection;
        }
        else
        {
            shootDirection = lastLookDirection; // fallback to last aim/move
        }

        shooter.TryFire(shootDirection);
    }

    private void AnimateMovement()
    {
        float moveMagnitude = moveInput.magnitude;

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetFloat("MoveMagnitude", moveMagnitude);

        if (moveMagnitude > 0.1f)
        {
            animator.SetFloat("LastMoveX", moveInput.x);
            animator.SetFloat("LastMoveY", moveInput.y);
        }
    }

    void FixedUpdate()
    {
        if (!controlsEnabled) return;

        Vector2 newPosition = rb.position + moveInput * GetCurrentSpeed() * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        AnimateMovement();
    }

    private float GetCurrentSpeed()
    {
        return baseSpeed + speedLevel * speedUpgradeStep;
    }

    public void IncreaseSpeedLevel()
    {
        speedLevel = Mathf.Min(speedLevel + 1, maxSpeedLevel);
        Debug.Log($"Speed level increased to {speedLevel}, current speed: {GetCurrentSpeed():0.00}");
    }
}
