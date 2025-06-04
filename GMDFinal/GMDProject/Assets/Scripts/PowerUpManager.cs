using UnityEngine;
using System;
using Random = UnityEngine.Random;

public enum PowerUpType
{
    Health,
    Damage,
    Speed,
    ProjectileCount,
    FireRate
}

public class PowerUpManager : MonoBehaviour
{
    private HealthComponent healthComponent = null;
    private DamageDealer damageDealer = null;
    private PlayerController playerController = null;
    private ProjectileShooter projectileShooter = null;

    private void OnEnable()
    {
        ExperienceManager.Instance.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        ExperienceManager.Instance.OnLevelUp -= HandleLevelUp;
    }

    public void RegisterPlayer(GameObject player)
    {
        healthComponent = player.GetComponent<HealthComponent>();
        damageDealer = player.GetComponent<DamageDealer>();
        playerController = player.GetComponent<PlayerController>();
        projectileShooter = player.GetComponent<ProjectileShooter>();
    }

    private void HandleLevelUp(int level)
    {
        ApplyRandomPowerUp();
    }

 private void ApplyRandomPowerUp()
{
    PowerUpType chosenType = (PowerUpType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(PowerUpType)).Length);

    switch (chosenType)
    {
        case PowerUpType.Health:
            healthComponent?.IncreaseHealthLevel();
            Debug.Log($"[POWERUP] Health level increased");
            break;

        case PowerUpType.Damage:
            damageDealer?.IncreaseDamageLevel();
            Debug.Log($"[POWERUP] Damage level increased.");
            break;

        case PowerUpType.Speed:
            playerController?.IncreaseSpeedLevel();
            Debug.Log($"[POWERUP] Speed level increased.");
            break;

        case PowerUpType.ProjectileCount:
            projectileShooter?.IncreaseProjectileLevel();
            Debug.Log($"[POWERUP] Projectile count level increased.");
            break;

        case PowerUpType.FireRate:
            projectileShooter?.IncreaseFireRateLevel();
            Debug.Log($"[POWERUP] Fire rate level increased.");
            break;
    }

    Debug.Log($"[POWERUP] Applied {chosenType}");
    }

}
