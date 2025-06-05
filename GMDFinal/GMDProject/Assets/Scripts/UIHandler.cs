using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

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

    private VisualElement m_UpgradePanel;
    private Button m_UpgradeButton1;
    private Button m_UpgradeButton2;

    private PowerUpType upgradeOption1;
    private PowerUpType upgradeOption2;
    private Action<PowerUpType> onUpgradeSelected;

    private bool isPaused = false;
    private InputSystem_Actions inputActions;

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

        m_PauseMenu = m_Root.Q<VisualElement>("PauseMenu");
        m_ContinueButton = m_PauseMenu.Q<Button>("continue-button");
        m_PauseMainMenuButton = m_PauseMenu.Q<Button>("pause-mainmenu-button");
        m_PauseQuitButton = m_PauseMenu.Q<Button>("pause-quit-button");

        m_UpgradePanel = m_Root.Q<VisualElement>("UpgradePanel");
        m_UpgradeButton1 = m_UpgradePanel.Q<Button>("upgrade-button-1");
        m_UpgradeButton2 = m_UpgradePanel.Q<Button>("upgrade-button-2");

        m_GameUI.style.display = DisplayStyle.None;
        m_DeathPopup.style.display = DisplayStyle.None;
        m_PauseMenu.style.display = DisplayStyle.None;
        m_UpgradePanel.style.display = DisplayStyle.None;

        m_PlayButton.clicked += OnPlayClicked;
        m_QuitButton.clicked += OnQuitClicked;
        m_DeathRestartButton.clicked += OnRestartClicked;
        m_DeathQuitButton.clicked += OnQuitClicked;

        m_ContinueButton.clicked += ResumeGame;
        m_PauseMainMenuButton.clicked += ReturnToMainMenu;
        m_PauseQuitButton.clicked += OnQuitClicked;

        m_UpgradeButton1.clicked += () => OnUpgradeSelected(upgradeOption1);
        m_UpgradeButton2.clicked += () => OnUpgradeSelected(upgradeOption2);

        m_PlayButton.Focus();

        inputActions = new InputSystem_Actions();
        inputActions.UI.Pause.performed += ctx => HandlePauseInput();
    }

    private void OnEnable()
    {
        inputActions ??= new InputSystem_Actions();
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        inputActions.UI.Disable();
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

        playerController?.EnableControls(false);
        enemySpawner?.SetSpawningEnabled(false);
    }

    private void HandlePauseInput()
    {
        if (isPaused)
            ResumeGame();
        else if (m_GameUI.style.display == DisplayStyle.Flex)
            PauseGame();
    }

    public void ShowMainMenu()
    {
        m_GameUI.style.display = DisplayStyle.None;
        m_DeathPopup.style.display = DisplayStyle.None;
        m_PauseMenu.style.display = DisplayStyle.None;
        m_MainMenu.style.display = DisplayStyle.Flex;

        m_Root.style.backgroundColor = new StyleColor(Color.black);

        playerController?.EnableControls(false);
        enemySpawner?.SetSpawningEnabled(false);

        SetExperienceValue(0f);
        SetPlayerLevel(1);
    }

    private void OnPlayClicked()
    {
        m_MainMenu.style.display = DisplayStyle.None;
        m_GameUI.style.display = DisplayStyle.Flex;

        m_Root.style.backgroundColor = new StyleColor(Color.clear);

        SetHealthValue(1.0f);

        GameManager.Instance.StartGame();
        playerController?.EnableControls(true);
        enemySpawner?.SetSpawningEnabled(true);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        m_PauseMenu.style.display = DisplayStyle.Flex;
        playerController?.EnableControls(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        m_PauseMenu.style.display = DisplayStyle.None;
        playerController?.EnableControls(true);
    }

    private void ReturnToMainMenu()
    {
        ResumeGame();
        GameManager.Instance.ResetGame();
        ShowMainMenu();
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

    public void SetHealthText(int current, int max)
    {
        if (m_HealthLabel != null)
        {
            m_HealthLabel.text = $"Health: {current} / {max}";
        }
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
        // Upgrade UI is now triggered by PowerUpManager
    }

    public void SetPlayerLevel(int level)
    {
        if (m_LevelLabel != null)
        {
            m_LevelLabel.text = $"Level: {level}";
        }
    }

    public void ShowDeathPopup()
    {
        m_DeathPopup.style.display = DisplayStyle.Flex;
    }

    public void HideDeathPopup()
    {
        m_DeathPopup.style.display = DisplayStyle.None;
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        isPaused = false;
        GameManager.Instance.ResetGame();
    }

    public void ShowUpgradeSelection(PowerUpType option1, PowerUpType option2, Action<PowerUpType> callback)
    {
        upgradeOption1 = option1;
        upgradeOption2 = option2;
        onUpgradeSelected = callback;

        // Schedule the UI update for the next frame (avoids layout pass conflict)
        m_UpgradePanel.schedule.Execute(() =>
        {
            m_UpgradeButton1.text = option1.ToString();
            m_UpgradeButton2.text = option2.ToString();

            m_UpgradePanel.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
            playerController?.EnableControls(false);
        });
    }


    private void OnUpgradeSelected(PowerUpType chosen)
    {
        m_UpgradePanel.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
        playerController?.EnableControls(true);

        onUpgradeSelected?.Invoke(chosen);
    }
}
