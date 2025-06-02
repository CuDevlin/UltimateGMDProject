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

    private VisualElement m_DeathPopup;
    private Button m_DeathRestartButton;
    private Button m_DeathQuitButton;

    private VisualElement m_PauseMenu;
    private Button m_ContinueButton;
    private Button m_PauseMainMenuButton;
    private Button m_PauseQuitButton;

    private bool isPaused = false;

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

        m_DeathPopup = m_Root.Q<VisualElement>("DeathPopup");
        m_DeathRestartButton = m_DeathPopup.Q<Button>("restart-button");
        m_DeathQuitButton = m_DeathPopup.Q<Button>("death-quit-button");

        // Pause menu elements
        m_PauseMenu = m_Root.Q<VisualElement>("PauseMenu");
        m_ContinueButton = m_PauseMenu.Q<Button>("continue-button");
        m_PauseMainMenuButton = m_PauseMenu.Q<Button>("pause-mainmenu-button");
        m_PauseQuitButton = m_PauseMenu.Q<Button>("pause-quit-button");

        // Set the GameUI, PauseMenu, and DeathPopup to be hidden initially.
        m_GameUI.style.display = DisplayStyle.None;
        m_DeathPopup.style.display = DisplayStyle.None;
        m_PauseMenu.style.display = DisplayStyle.None;

        // Assign button click events
        m_PlayButton.clicked += OnPlayClicked;
        m_QuitButton.clicked += OnQuitClicked;
        m_DeathRestartButton.clicked += OnRestartClicked;
        m_DeathQuitButton.clicked += OnQuitClicked;

        m_ContinueButton.clicked += ResumeGame;
        m_PauseMainMenuButton.clicked += ReturnToMainMenu;
        m_PauseQuitButton.clicked += OnQuitClicked;

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

    private void Update()
    {
        // Toggle pause menu with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else if (m_GameUI.style.display == DisplayStyle.Flex)
                PauseGame();
        }
    }

    public void ShowMainMenu()
    {
        // Hide the GameUI, PauseMenu, and DeathPopup, show MainMenu
        m_GameUI.style.display = DisplayStyle.None;
        m_DeathPopup.style.display = DisplayStyle.None;
        m_PauseMenu.style.display = DisplayStyle.None;
        m_MainMenu.style.display = DisplayStyle.Flex;

        // Set the background color of the root to black when in main menu
        m_Root.style.backgroundColor = new StyleColor(Color.black);

        // Disable player controls and enemy spawner while in the main menu
        if (playerController != null)
            playerController.EnableControls(false);

        if (enemySpawner != null)
            enemySpawner.SetSpawningEnabled(false);

        // Reset experience bar and level to default
        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetExperienceValue(0f);  // Reset experience bar to 0%
            UIHandler.instance.SetPlayerLevel(1);      // Reset level label to 1
        }
    }

    private void OnPlayClicked()
    {
        // Hide MainMenu and show GameUI
        m_MainMenu.style.display = DisplayStyle.None;
        m_GameUI.style.display = DisplayStyle.Flex;

        // Remove the black background when transitioning to the game
        m_Root.style.backgroundColor = new StyleColor(Color.clear);

        // Initialize health bar to full (100%)
        SetHealthValue(1.0f);

        // Start the game through GameManager
        GameManager.Instance.StartGame();

        // Enable player controls and enemy spawner when the game starts
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

    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        m_PauseMenu.style.display = DisplayStyle.Flex;

        if (playerController != null)
            playerController.EnableControls(false);
    }

    // Resume the game from pause
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        m_PauseMenu.style.display = DisplayStyle.None;

        if (playerController != null)
            playerController.EnableControls(true);
    }

    // Return to main menu from pause
    private void ReturnToMainMenu()
    {
        ResumeGame(); // Ensure time resumes

        // Reset game state fully
        GameManager.Instance.ResetGame();

        ShowMainMenu();
    }

    // Set Health Bar Value
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

    // Set Experience Bar Value
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

    // Update Max Health Bar width
    public void UpdateMaxHealth(int newMaxHealth)
    {
        maxHealthBarWidth = Mathf.Max(newMaxHealth * 20, 300f);
        m_HealthBarContainer.style.width = maxHealthBarWidth;
    }

    // Update Health Text
    public void SetHealthText(int current, int max)
    {
        if (m_HealthLabel != null)
        {
            m_HealthLabel.text = $"Health: {current} / {max}";
        }
    }

    // Handle Experience Change
    private void OnExperienceChanged(int currentXP)
    {
        float percent = currentXP / (float)ExperienceManager.Instance.experienceToNextLevel;
        SetExperienceValue(percent);
    }

    // Handle Level Up
    private void OnLevelUp(int newLevel)
    {
        SetExperienceValue(0);  // Reset experience bar on level-up
        SetPlayerLevel(newLevel);  // Update the level label
    }

    // Set Player Level Label
    public void SetPlayerLevel(int level)
    {
        if (m_LevelLabel != null)
        {
            m_LevelLabel.text = $"Level: {level}";
        }
    }

    // Show Death Popup
    public void ShowDeathPopup()
    {
        if (m_DeathPopup != null)
        {
            m_DeathPopup.style.display = DisplayStyle.Flex; // Show death popup
        }
    }

    // Hide Death Popup (called when restarting the game)
    public void HideDeathPopup()
    {
        if (m_DeathPopup != null)
        {
            m_DeathPopup.style.display = DisplayStyle.None; // Hide death popup
        }
    }

    // Restart the game (reset to main menu)
    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        isPaused = false;
        GameManager.Instance.ResetGame();
    }
}