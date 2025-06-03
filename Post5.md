# UltimateGMDProject Development Blog 3

Devlin Onichuk

02 06 2025
# Description

## Expanding Core Systems: UI Menus, Game States, Animations, and Audio

This development phase pushed our project beyond the foundational gameplay mechanics by adding critical polish and functionality that enhance player control, clarity, and immersion. Major improvements include a robust **Pause Menu**, better **game state management**, detailed **8-directional animations**, and an integrated **audio system** for background music and sound effects.

These additions not only improve the moment-to-moment experience but lay the groundwork for a more scalable and professional arcade game framework.

---

##  Game State Control and UI Menus

### Pause Menu

A key feature introduced this sprint is the ability to **pause the game** using the `Escape` key. This displays a Pause Menu via Unity’s **UI Toolkit** with three core options:

- **Continue** — Resumes gameplay instantly.
- **Main Menu** — Returns to the title screen and resets game state.
- **Quit** — Exits the game or editor play mode.

This menu was built to match the existing Game UI style, maintaining a cohesive aesthetic.

**Why It Matters**:  
Pausing is fundamental for user control and accessibility. Players should be able to take a break, adjust settings, or exit without penalty. The system works by toggling `Time.timeScale` between `1` and `0`, effectively freezing all time-based gameplay while keeping UI elements responsive.

```csharp
public void TogglePause()
{
    if (isPaused) ResumeGame();
    else PauseGame();
}
```
The PauseManager handles this logic cleanly, avoiding clutter in the main gameplay or UI scripts.

## Proper Main Menu Reset
Previously, returning to the Main Menu did not reset gameplay properly — resulting in issues like:

A second player being spawned when restarting

Enemies continuing to spawn

Projectiles still floating or flying around

This was resolved by expanding GameManager.ResetGame() to:

Stop enemy spawning and destroy all enemies

Clear all active projectiles

Disable and reposition the player

Reset player stats (health, experience, level)

Hide gameplay UI and show the Main Menu

Reset audio

**Why It Matters:**
Resetting the game state ensures the player always starts fresh. Without this, each session becomes unpredictable and chaotic, especially when testing or demoing the game. It also prevents memory leaks or gameplay logic errors from stacking between playthroughs.

## 8-Directional Animations
The player now has 8-directional idle and walk animations — covering N, NE, E, SE, S, SW, W, and NW directions. This is accomplished using an Animator Controller with blend trees, driven by the movement vector.

```csharp
animator.SetFloat("MoveX", moveInput.x);
animator.SetFloat("MoveY", moveInput.y);
```
When no input is given, the character idles in the last moved direction, allowing directional aiming and consistent visual feedback.

**Why It Matters:**
Without proper directional animation, top-down movement can feel unresponsive or generic. By animating all 8 directions, movement becomes more readable and fluid, especially during fast-paced enemy encounters. It also supports future polish like directional attacks or aim-based mechanics.

**Audio: Music and Sound Effects**
**Background Music**
The game now plays a looping music track during gameplay. The music stops when the game is paused or the player returns to the main menu, avoiding jarring overlaps.

Uses a persistent AudioSource

Fade in/out will be added later for smoother transitions

**Projectile Firing Sound**
When firing a projectile, a short sound clip plays using AudioSource.PlayOneShot(). This is triggered inside ProjectileShooter when the player successfully fires.

```csharp
audioSource.PlayOneShot(shootClip);
Why It Matters:
```
Audio cues are essential for both feedback and immersion. The background music sets the tone of gameplay, while shooting sounds confirm player input and enhance satisfaction. In the future, more SFX will accompany hits, level-ups, and menu interactions.

## Summary of New Features
Feature	Purpose & Benefit
Pause Menu	Enables pausing gameplay without exiting; adds player agency and polish.
Main Menu Reset	Ensures consistent, clean game starts; fixes bugs with extra spawns or projectiles.
8-Direction Anim	Improves character clarity; supports better combat feedback and polish.
SFX / Music	Adds atmosphere and responsive audio feedback for core actions.
