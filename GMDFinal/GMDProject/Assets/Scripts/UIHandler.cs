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

    private Button m_PlayButton;
    private Button m_QuitButton;

    private float maxHealthBarWidth = 300f; // Set the maximum width for health bar container
    private float minHealthBarWidth = 100f; // Set a minimum width for the health bar container

    private void Awake()
    {
        // Singleton pattern
        instance = this;

        // Get the UIDocument
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is not attached to the GameObject.");
            return;
        }

        // Root UI element
        m_Root = uiDocument.rootVisualElement;

        // Get UI containers
        m_MainMenu = m_Root.Q<VisualElement>("MainMenu");
        m_GameUI = m_Root.Q<VisualElement>("GameUI");

        // Get health bar elements
        m_HealthBarContainer = m_Root.Q<VisualElement>("HealthBarContainer");
        m_HealthBarFill = m_HealthBarContainer.Q<VisualElement>("HealthBarFill");

        // Set initial visibility
        m_MainMenu.style.display = DisplayStyle.Flex;
        m_GameUI.style.display = DisplayStyle.None;

        // Get buttons
        m_PlayButton = m_MainMenu.Q<Button>("play-button");
        m_QuitButton = m_MainMenu.Q<Button>("quit-button");

        // Setup button callbacks
        m_PlayButton.clicked += OnPlayClicked;
        m_QuitButton.clicked += OnQuitClicked;

        // Set initial focus to Play button
        m_PlayButton.Focus();
    }

    private void OnPlayClicked()
    {
        m_MainMenu.style.display = DisplayStyle.None;
        m_GameUI.style.display = DisplayStyle.Flex;

        // Reset health bar
        SetHealthValue(1.0f);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Update health bar fill
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

    // Update container width based on max health
    public void UpdateMaxHealth(int newMaxHealth)
    {
        maxHealthBarWidth = Mathf.Max(newMaxHealth * 20, 300f);
        m_HealthBarContainer.style.width = maxHealthBarWidth;
    }
}