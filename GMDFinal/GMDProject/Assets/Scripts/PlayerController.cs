using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   // Variables related to player character movement
   private Rigidbody2D rigidbody2d;
   private Vector2 moveInput;
   public float speed = 3.0f;
   private Vector2 lastMoveDirection = Vector2.up; // Default direction is up (when idle)
   private float projectileSpeed = 6f;

   // Variables related to the health system
   public int maxHealth = 5;
   private int currentHealth;
   public int health { get { return currentHealth; }}

   // Variables related to temporary invincibility
   public float timeInvincible = 2.0f;
   private bool isInvincible;
   private float damageCooldown;

  // Variables related to projectiles
   public GameObject projectilePrefab;
   public float fireRate = 0.5f; // Time between shots in seconds
   private float fireCooldown = 1f; // Tracks the time left before the player can fire again

   // Start is called before the first frame update
   void Awake()
   {
      rigidbody2d = GetComponent<Rigidbody2D>();
      currentHealth = maxHealth;
   }
 
  // Called when Move input is detected
   public void OnMove(InputValue value)
   {
   moveInput = value.Get<Vector2>();

    // Update the last move direction, only if the player is moving
      if (moveInput != Vector2.zero)
   {
      lastMoveDirection = moveInput.normalized; // Store the normalized direction
   }
   }
  // Called when Fire input is detected
  public void OnFire()
   {
      if (fireCooldown <= 0f)  // Only fire if the cooldown has expired
      {
         Launch();
         fireCooldown = fireRate;  // Reset the cooldown after firing
      }
   }
  // FixedUpdate has the same call rate as the physics system
  void FixedUpdate()
  {
     Vector2 position = (Vector2)rigidbody2d.position + moveInput * speed * Time.deltaTime;
     rigidbody2d.MovePosition(position);
  }

   void Update()
   {
      // Decrease the cooldown over time
      if (fireCooldown > 0f)
      {
         fireCooldown -= Time.deltaTime;
      }
   }

  public void ChangeHealth (int amount)
  {
     if (amount < 0)
     {
         if (isInvincible)
             return;
        
         isInvincible = true;
         damageCooldown = timeInvincible;
     }

     currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
     UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
  }

   void Launch()
   {
      GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
      Projectile projectile = projectileObject.GetComponent<Projectile>();

      // Use lastMoveDirection if idle, otherwise use the current movement input
      Vector2 launchDirection = moveInput != Vector2.zero ? moveInput.normalized : lastMoveDirection;

      // Apply force to the projectile
      projectile.Launch(launchDirection, projectileSpeed);
   }
}