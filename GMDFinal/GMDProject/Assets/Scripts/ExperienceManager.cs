using UnityEngine;
using System;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    [Header("Experience Settings")]
    public int currentExperience = 0;
    public int currentLevel = 1;
    public int experienceToNextLevel = 100;
    public float experienceGrowthRate = 1.2f;

    public event Action<int> OnLevelUp;
    public event Action<int> OnExperienceChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Debug.Log("ExperienceManager instance set.");
        Instance = this;
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        OnExperienceChanged?.Invoke(currentExperience);

        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            LevelUp();
        }

        Debug.Log($"Current Experience: {currentExperience}, Level: {currentLevel}");
    }

    private void LevelUp()
    {
        currentLevel++;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * experienceGrowthRate);
        OnLevelUp?.Invoke(currentLevel);
    }
}
