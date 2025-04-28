using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
   private VisualElement m_Healthbar;
   public static UIHandler instance { get; private set; }

   // Awake is called when the script instance is being loaded (in this situation, when the game scene loads)
   private void Awake()
   {
       instance = this;
       UIDocument uiDocument = GetComponent<UIDocument>();
       m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
   }

   // Start is called before the first frame update
   void Start()
   {       
       SetHealthValue(1.0f);
   }

   public void SetHealthValue(float percentage)
   {
       m_Healthbar.style.width = Length.Percent(100 * percentage);
   }
}