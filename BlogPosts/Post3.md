# UltimateGMDProject Development Blog 1

Devlin Onichuk

28 03 2025
## Description

### Developing Core Mechanics: Player and Enemy Systems
This phase of our development focuses on implementing the main features and core game mechanics. This includes refining player movement and actions, setting up controls to fit the arcade machine that will be used, and ensuring they integrate smoothly with Unity’s new input system. Additionally, we worked on enemy movement, particularly how they follow the player, and implemented an enemy spawning system that ensures they appear in specific areas on the map. Below, we’ll discuss how these systems were implemented.

### Player Movement & Controls
The player movement system was built using Unity’s new Input System, allowing us to support multiple control schemes, including WASD, arrow keys, and a joystick for arcade machines. Instead of relying on Unity’s old Input system (Input.GetAxis), we used PlayerInput to manage input detection and responses dynamically.

The movement is handled in FixedUpdate() to keep it in sync with Unity’s physics engine, ensuring smoother motion. The movement vector updates based on player input and moves the Rigidbody2D:

---
```csharp
void FixedUpdate()
{
    Vector2 position = (Vector2)rigidbody2d.position + moveInput * speed * Time.deltaTime;
    rigidbody2d.MovePosition(position);
}
```
---
One of the key improvements was allowing the player to fire projectiles in the direction they last moved, ensuring that when stationary, projectiles still fire in the intended direction.

### Projectile System & Fire Rate Control
Projectiles were implemented using Rigidbody2D, moving in the direction of fire using velocity. A firing cooldown system was added to prevent spamming:

---
```csharp
private float fireCooldown = 0.5f;
private float nextFireTime = 0f;

void OnFire()
  {
      if (Time.time >= nextFireTime)
      {
          Launch();  
          nextFireTime = Time.time + fireCooldown; 
      }    
  }
```
---

This ensures that players must wait before firing again, balancing gameplay.

### Enemy AI: Movement & Health System
Enemies were designed to follow the player using a simple AI system. Each enemy calculates a movement vector based on the player’s position:

```csharp
Vector2 direction = (player.position - transform.position).normalized;
rigidbody2d.MovePosition(rigidbody2d.position + direction * speed * Time.deltaTime);
```

For combat, we implemented a health system where enemies take damage upon being hit by projectiles. Initially, enemies were being destroyed immediately upon collision, due to the damage function being called multiple times. This was fixed by ensuring that TakeDamage() is only called once per hit, and knockback effects were added to enhance the impact of projectile hits.

### Enemy Spawning System
To keep the game engaging, we implemented an enemy spawn system that ensures enemies appear only in specific locations. Instead of randomly placing them, we set up designated spawn points and used a spawn manager to control when and where new enemies appear.

---
```csharp
void SpawnEnemy()
{
    Vector2 spawnLocation = spawnPoints[Random.Range(0, spawnPoints.Length)].position;   
    Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
}
```
---

This system ensures that enemies don’t spawn unfairly close to the player while keeping the difficulty balanced.

### Camera Follow System
Finally, we added a camera follow system using Unity’s Cinemachine to smoothly track the player’s movement. This keeps the player centered while allowing for a dynamic view of the game world.

These implementations form the foundation of our gameplay mechanics, setting up smooth controls, engaging enemy interactions, and structured enemy spawning. In the next phase, we’ll focus on expanding these systems with more advanced AI behaviors, power-ups, and additional mechanics to enhance the player experience.
