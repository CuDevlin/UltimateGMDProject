using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnemySpawner enemySpawner;
    private VisualElement m_Root;
    private VisualElement m_MainMenu;
    private VisualElement m_GameUI;

    private VisualElement m_HealthBarContainer;
    private VisualElement m_HealthBarFill;

    private VisualElement m_ExpBarContainer;
    private VisualElement m_ExpBarFill;

    private Button m_PlayButton;
    private Button m_QuitButton;

    private float maxHealthBarWidth = 300f;
    private Label m_LevelLabel;
    private Label m_HealthLabel;

    private void Awake()
    {
        instance = this;

        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is not attached to the GameObject.");
            return;
        }

        m_Root = uiDocument.rootVisualElement;

        m_MainMenu = m_Root.Q<VisualElement>("MainMenu");
        m_GameUI = m_Root.Q<VisualElement>("GameUI");

        m_HealthBarContainer = m_Root.Q<VisualElement>("HealthBarContainer");
        m_HealthBarFill = m_HealthBarContainer.Q<VisualElement>("HealthBarFill");

        m_ExpBarContainer = m_Root.Q<VisualElement>("ExpBarContainer");
        m_ExpBarFill = m_ExpBarContainer.Q<VisualElement>("ExpBarFill");

        m_PlayButton = m_MainMenu.Q<Button>("play-button");
        m_QuitButton = m_MainMenu.Q<Button>("quit-button");

        m_LevelLabel = m_Root.Q<Label>("LevelLabel");
        m_HealthLabel = m_Root.Q<Label>("HealthLabel");

        m_PlayButton.clicked += OnPlayClicked;
        m_QuitButton.clicked += OnQuitClicked;
        m_PlayButton.Focus();
    }

    private void Start()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChanged += OnExperienceChanged;
            ExperienceManager.Instance.OnLevelUp += OnLevelUp;

            SetPlayerLevel(ExperienceManager.Instance.currentLevel);
            OnExperienceChanged(ExperienceManager.Instance.currentExperience);
        }

        if (playerController != null)
            playerController.EnableControls(false);

        if (enemySpawner != null)
            enemySpawner.SetSpawningEnabled(false);

    }

    private void OnPlayClicked()
    {
        m_MainMenu.style.display = DisplayStyle.None;
        m_GameUI.style.display = DisplayStyle.Flex;

        SetHealthValue(1.0f);

        if (playerController != null)
            playerController.EnableControls(true);

        if (enemySpawner != null)
            enemySpawner.SetSpawningEnabled(true);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetHealthValue(float percentage)
    {
        if (m_HealthBarFill != null && m_HealthBarContainer != null)
        {
            percentage = Mathf.Clamp01(percentage);
            m_HealthBarFill.style.width = Length.Percent(100 * percentage);
        }
        else
        {
            Debug.LogError("HealthBar elements not found!");
        }
    }

    public void SetExperienceValue(float percentage)
    {
        Debug.Log($"[EXP BAR] Setting experience bar to: {percentage * 100}%");

        if (m_ExpBarFill != null && m_ExpBarContainer != null)
        {
            percentage = Mathf.Clamp01(percentage);
            m_ExpBarFill.style.width = Length.Percent(percentage * 100);
        }
        else
        {
            Debug.LogError("EXP Bar elements not found!");
        }
    }

    public void UpdateMaxHealth(int newMaxHealth)
    {
        maxHealthBarWidth = Mathf.Max(newMaxHealth * 20, 300f);
        m_HealthBarContainer.style.width = maxHealthBarWidth;
    }

    private void OnExperienceChanged(int currentXP)
    {
        float percent = currentXP / (float)ExperienceManager.Instance.experienceToNextLevel;
        SetExperienceValue(percent);
    }

        private void OnLevelUp(int newLevel)
    {
        SetExperienceValue(0);
        SetPlayerLevel(newLevel);
    }

    public void SetPlayerLevel(int level)
    {
        if (m_LevelLabel != null)
        {
            m_LevelLabel.text = $"Level: {level}";
        }
    }

    public void SetHealthText(int current, int max)
    {
        if (m_HealthLabel != null)
        {
            m_HealthLabel.text = $"Health: {current} / {max}";
        }
    }
}