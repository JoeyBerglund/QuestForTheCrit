using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TargetButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Button targetButton;  // Reference to the button that is being hovered
    public Text buttonText;      // Optional: Text that can change when the button is selected
    private Vector3 originalScale; // Store the original scale for reset

    // Optional: Color change when hovered
    public Color hoverColor = Color.yellow; 
    public Color normalColor = Color.white;

    public EnemyController targetEnemy;  // The enemy this button represents (set in the inspector)
    public CombatManager combatManager;  // Reference to the CombatManager to update the target
    
    // Set up initial values
    void Start()
    {
        if (targetButton != null)
        {
            originalScale = targetButton.transform.localScale;  // Save the original scale of the button
        }
    }

    // When the pointer enters the button's area
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetButton != null)
        {
            // Increase size of the button when hovered over
            targetButton.transform.localScale = originalScale * 1.1f;
            // Change button color to hover color
            targetButton.GetComponent<Image>().color = hoverColor;

            // Optional: Change button text when hovered
            if (buttonText != null)
            {
                buttonText.text = "Select Target";  // You can modify this as needed
            }
        }
    }

    // When the pointer exits the button's area
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetButton != null)
        {
            // Reset size of the button to its original scale
            targetButton.transform.localScale = originalScale;
            // Reset button color to normal color
            targetButton.GetComponent<Image>().color = normalColor;

            // Optional: Reset button text when not hovered
            if (buttonText != null)
            {
                buttonText.text = "";  // Reset or put default text
            }
        }
    }

    // When the button is clicked (select target)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetButton != null && combatManager != null && targetEnemy != null)
        {
            // Set the selected target in the combat manager
            combatManager.SetTarget(targetEnemy);
            
            // Optionally: Update button text when selected
            if (buttonText != null)
            {
                buttonText.text = "Target Selected!";
            }
        }
    }
}
