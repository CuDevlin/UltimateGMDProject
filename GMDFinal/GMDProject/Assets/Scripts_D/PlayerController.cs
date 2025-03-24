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
  }

  // Called when Fire input is detected
  public void OnFire()
  {
      Launch();
  }

  // FixedUpdate has the same call rate as the physics system
  void FixedUpdate()
  {
     Vector2 position = (Vector2)rigidbody2d.position + moveInput * speed * Time.deltaTime;
     rigidbody2d.MovePosition(position);
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

     currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
     UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
  }

   void Launch()
   {
      GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
      Projectile projectile = projectileObject.GetComponent<Projectile>();

      // Set direction based on player movement input (or default upward if idle)
      Vector2 launchDirection = moveInput != Vector2.zero ? moveInput : Vector2.up;
      
      // Apply force to the projectile
      projectile.Launch(launchDirection, projectileSpeed);
   }
}