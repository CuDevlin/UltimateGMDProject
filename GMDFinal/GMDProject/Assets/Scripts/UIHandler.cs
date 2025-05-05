using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    private VisualElement m_Root;
    private VisualElement m_MainMenu;
    private VisualElement m_GameUI;
    private VisualElement m_Healthbar;
    private VisualElement m_FadeOverlay;

    private const float FADE_DURATION = 0.5f;

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

        // Query Game UI elements like the HealthBar and FadeOverlay
        m_Healthbar = m_Root.Q<VisualElement>("HealthBar");
        m_FadeOverlay = m_Root.Q<VisualElement>("FadeOverlay");

        // Initial visibility settings
        m_MainMenu.style.display = DisplayStyle.Flex;  // Show MainMenu
        m_GameUI.style.display = DisplayStyle.None;    // Hide GameUI initially
        m_FadeOverlay.style.opacity = 0;  // Hide fade overlay initially

        // Setup button events
        m_MainMenu.Q<Button>("play-button").clicked += OnPlayClicked;
        m_MainMenu.Q<Button>("quit-button").clicked += OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        // Start fade and UI switch when play is clicked
        StartCoroutine(FadeAndSwitchUI());
    }

    private IEnumerator FadeAndSwitchUI()
    {
        // Show the fade-to-black overlay
        m_FadeOverlay.AddToClassList("fade-visible");
        yield return new WaitForSeconds(FADE_DURATION);

        // Switch UIs: Hide MainMenu, Show GameUI
        m_MainMenu.style.display = DisplayStyle.None;
        m_GameUI.style.display = DisplayStyle.Flex;

        // Optionally reset the health bar
        SetHealthValue(1.0f);

        // Fade out the black overlay (to reveal GameUI)
        m_FadeOverlay.RemoveFromClassList("fade-visible");
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
        if (m_Healthbar != null)
        {
            m_Healthbar.style.width = Length.Percent(100 * percentage);  // Update the health bar width
        }
        else
        {
            Debug.LogError("HealthBar element not found!");
        }
    }
}