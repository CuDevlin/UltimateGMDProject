# UltimateGMDProject Development Blog 2

Adrian Bugiel

05-06-2025

## Developing progression & scaling systems

In *VIA Survivors*, player progression is a core mechanic that sustains difficulty scaling and strategic variety throughout the game. The system leverages simple design principles to deliver compelling choices through well-structured level-ups and dynamically scaling enemy quantities.

## Leveling: The core mechanism

The main component of the progression system is the `ExperienceManager`, a singleton class responsible for:

* Tracking current experience and level
* Determining the XP threshold for leveling up (increasing 1,2 times per level)
* Emitting events on new level gained

```csharp
public void GainExperience(int amount)
{
    currentExperience += amount;
    OnExperienceChanged?.Invoke(currentExperience);

    while (currentExperience >= experienceToNextLevel)
    {
        currentExperience -= experienceToNextLevel;
        LevelUp();
    }
}
```

Each level-up triggers the `OnLevelUp` event, which is picked up by the `PowerUpManager` to initiate a player upgrade choice.

## Power-Ups: Upgrades on every level

Upon each level-up, the player is presented with a random choice of **two upgrades** from the following types:

* `Health`
* `Damage`
* `Speed`
* `ProjectileCount`
* `FireRate`

```csharp
PowerUpType option1 = allOptions[Random.Range(0, allOptions.Count)];
PowerUpType option2;
do {
    option2 = allOptions[Random.Range(0, allOptions.Count)];
} while (option2 == option1);
```

These are presented to the UI via `UIHandler.instance.ShowUpgradeSelection`, giving the player control over their build direction. Each upgrade type supports **10 levels of scaling**, capped to prevent overgrowth but deep enough for meaningful progression.

For instance the fire rate has a cooldown of 0.6 seconds and is improved by 0.05 seconds on each chosen level:

```csharp
public void IncreaseFireRateLevel()
{
    fireRateLevel = Mathf.Min(fireRateLevel + 1, maxFireRateLevel);
    Debug.Log($"Fire rate level increased to {fireRateLevel}");
}
```

---

## Passive Scaling: Health Boost on Level-Up

Even when not selecting a health power-up, the `HealthComponent` grants a **+1 flat HP boost** on every level and returns player's health to maximum level.

This ensures the player is always becoming slightly more resilient, even without direct defensive choices and has a chance to regenerate.


## Difficulty Scaling via Enemy Spawning

To match the player's growing power:

* Enemy caps increase every fixed interval (currently every 20s) by 1
* Each enemy prefab can be added via `EnemySpawner` and set to initial cap value
* For example the starting caps could be set to 20 for prefab A, 10 for prefab B, and 5 for prefab C

```csharp
if (limitTimer >= limitIncreaseInterval)
{
    currentLimitA += limitIncreaseAmount;
    currentLimitB += limitIncreaseAmount;
    currentLimitC += limitIncreaseAmount;
    limitTimer = 0f;
}
```

This keeps the pressure proportional by having numerous basic enemies and a couple stronger ones.