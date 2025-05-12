using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

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

        m_PlayButton.clicked += OnPlayClicked;
        m_QuitButton.clicked += OnQuitClicked;
        m_PlayButton.Focus();
    }

    private void Start()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChanged += OnExperienceChanged;
            
            // Immediately sync UI to current experience
            OnExperienceChanged(ExperienceManager.Instance.currentExperience);
        }
    }

    private void OnPlayClicked()
    {
        m_MainMenu.style.display = DisplayStyle.None;
        m_GameUI.style.display = DisplayStyle.Flex;

        SetHealthValue(1.0f);
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
        if (m_ExpBarFill != null && m_ExpBarContainer != null)
        {
            percentage = Mathf.Clamp01(percentage);
            m_ExpBarFill.style.width = Length.Percent(100 * percentage);
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

    private void OnExperienceChanged(int currentExp)
    {
        if (ExperienceManager.Instance != null)
        {
            float percent = (float)currentExp / ExperienceManager.Instance.experienceToNextLevel;
            SetExperienceValue(percent);
        }
    }
}