using UnityEngine;
using System;
using System.Collections.Generic;
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
        ShowUpgradeChoice();
    }

    private void ShowUpgradeChoice()
    {
        List<PowerUpType> allOptions = new List<PowerUpType>((PowerUpType[])Enum.GetValues(typeof(PowerUpType)));

        PowerUpType option1 = allOptions[Random.Range(0, allOptions.Count)];
        PowerUpType option2;

        do
        {
            option2 = allOptions[Random.Range(0, allOptions.Count)];
        } while (option2 == option1);

        UIHandler.instance.ShowUpgradeSelection(option1, option2, ApplyPowerUp);
    }

    private void ApplyPowerUp(PowerUpType chosenType)
    {
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
