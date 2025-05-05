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

    private float maxHealthBarWidth = 300f; // Set the maximum width for health bar container
    private float minHealthBarWidth = 100f; // Set a minimum width for the health bar container

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of UIHandler
        instance = this;

        // Get the UIDocument component attached to the same GameObject
        UIDocument uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is not attached to the GameObject.");
            return;
        }

        // Initialize the root VisualElement from the UIDocument
        m_Root = uiDocument.rootVisualElement;

        // Query the MainMenu and GameUI from the UXML
        m_MainMenu = m_Root.Q<VisualElement>("MainMenu");
        m_GameUI = m_Root.Q<VisualElement>("GameUI");

        // Query the health bar container and fill
        m_HealthBarContainer = m_Root.Q<VisualElement>("HealthBarContainer");
        m_HealthBarFill = m_HealthBarContainer.Q<VisualElement>("HealthBarFill");

        // Initial visibility settings
        m_MainMenu.style.display = DisplayStyle.Flex;  // Show MainMenu
        m_GameUI.style.display = DisplayStyle.None;    // Hide GameUI initially

        // Setup button events
        m_MainMenu.Q<Button>("play-button").clicked += OnPlayClicked;
        m_MainMenu.Q<Button>("quit-button").clicked += OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        // Hide Main Menu
        m_MainMenu.style.display = DisplayStyle.None;

        // Show Game UI
        m_GameUI.style.display = DisplayStyle.Flex;

        // Reset health bar to full (100%)
        SetHealthValue(1.0f);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Set the health bar fill percentage
    public void SetHealthValue(float percentage)
    {
        if (m_HealthBarFill != null && m_HealthBarContainer != null)
        {
            // Ensure health percentage is between 0 and 1
            percentage = Mathf.Clamp01(percentage);

            // Update the fill width based on the health percentage
            m_HealthBarFill.style.width = Length.Percent(100 * percentage);
        }
        else
        {
            Debug.LogError("HealthBar elements not found!");
        }
    }

    // Method to update the max health bar size (only changes the container width)
    public void UpdateMaxHealth(int newMaxHealth)
    {
        // Adjust the max width based on new max health value
        maxHealthBarWidth = Mathf.Max(newMaxHealth * 20, 300f); // Adjust multiplier if needed

        // Update the health bar container's width based on the new max health
        m_HealthBarContainer.style.width = maxHealthBarWidth;
    }
}