using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public PlayerController player;  // Reference to PlayerController
    public EnemyController enemy;    // Reference to EnemyController

    // UI elements
    public Text feedbackText;
    public Image playerHealthBar;
    public Image enemyHealthBar;
    public Button basicAttackButton;
    public Button skillAttackButton;
    public Button ultimateAttackButton;
    public Image ultimateButtonImage;

    public Text turnManager;  // Reference to turn manager

    private bool playerTurn = true;  // Track whose turn it is
    private bool isCombatOver = false;  // Track if combat is over

    void Start()
    {
        // Button listeners
        basicAttackButton.onClick.AddListener(() => PlayerAttack("Basic"));
        skillAttackButton.onClick.AddListener(() => PlayerAttack("Skill"));
        ultimateAttackButton.onClick.AddListener(() => PlayerAttack("Ultimate"));

        // Initial setup
        UpdateTurnOrder("Your turn!");
        UpdateHealthBars();
        UpdateUltimateButtonColor();  // Set initial color for ultimate button

        // Disable combat buttons initially for enemy's turn
        DisableCombatButtons(false);

        // Start the turn system
        StartTurn();
    }

    void StartTurn()
    {
        if (isCombatOver) return;  // If combat is over, do nothing
        
        if (playerTurn)
        {
            // It's the player's turn
            EnableCombatButtons(true);  // Enable the combat buttons for the player
            UpdateTurnOrder("Your turn!");
        }
        else
        {
            // It's the enemy's turn
            EnableCombatButtons(false);  // Disable the combat buttons for the player
            UpdateTurnOrder("Enemy's turn!");

            // Delay for enemy's turn to complete
            Invoke("EnemyTurn", 1.0f); // Let the enemy attack after a short delay
        }
    }

    void PlayerAttack(string attackType)
    {
        if (!playerTurn || !player.isAlive) return;

        switch (attackType)
        {
            case "Basic":
                player.BasicAttack(enemy, this);
                break;
            case "Skill":
                player.SkillAttack(enemy, this);
                break;
            case "Ultimate":
                player.UltimateAttack(enemy, this);
                break;
        }

        // Update energy after attack
        player.GainEnergy(20); // Example energy gain after action
        UpdateHealthBars();
        UpdateUltimateButtonColor();  // Update ultimate button after using energy
        playerTurn = false;

        // Transition to enemy's turn
        StartTurn();
    }

    void EnemyTurn()
    {
        if (enemy.isAlive)
        {
            enemy.BasicAttack(player, this);
        }

        // Check if the player is still alive after the enemy attack
        if (player.isAlive)
        {
            playerTurn = true;  // Switch turn to player
            StartTurn();  // Call StartTurn to update the UI and enable buttons for player
        }
        else
        {
            // Combat is over
            isCombatOver = true;
            UpdateFeedback("You have been defeated!");
            DisableCombatButtons(true); // Disable combat buttons
        }

        // Update health bars for both players
        UpdateHealthBars();
    }

    // Update health bars in the UI
    void UpdateHealthBars()
    {
        playerHealthBar.fillAmount = (float)player.health / 100;
        enemyHealthBar.fillAmount = (float)enemy.health / 100;
    }

    // Update feedback text for UI
    public void UpdateFeedback(string message)
    {
        feedbackText.text = message;
    }

     public void UpdateTurnOrder(string message)
    {
        turnManager.text = message;
    }

    // Update the ultimate button color based on energy
    void UpdateUltimateButtonColor()
    {
        float fillAmount = (float)player.energy / player.maxEnergy;
        ultimateButtonImage.fillAmount = fillAmount;  // Set fill amount based on player's energy
    }

    // Enable or disable combat buttons
    void EnableCombatButtons(bool enable)
    {
        basicAttackButton.interactable = enable;
        skillAttackButton.interactable = enable;
        ultimateAttackButton.interactable = enable;
    }

    // Disable all combat buttons (for enemy's turn)
    void DisableCombatButtons(bool disable)
    {
        basicAttackButton.interactable = !disable;
        skillAttackButton.interactable = !disable;
        ultimateAttackButton.interactable = !disable;
    }
}