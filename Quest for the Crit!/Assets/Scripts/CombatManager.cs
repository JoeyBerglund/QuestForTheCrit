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
    public Text skillPointsText;
    public Text turnManager;  // Reference to turn manager

    private bool playerTurn = true;  // Track whose turn it is
    private bool isCombatOver = false;  // Track if combat is over

    void Start()
    {
        // Button listeners
        basicAttackButton.onClick.AddListener(() => PlayerAttack("Basic"));
        skillAttackButton.onClick.AddListener(() => PlayerAttack("Skill"));
        ultimateAttackButton.onClick.AddListener(() => PlayerAttack("Ultimate"));

        // Roll for initiative at the start
        enemy.RollInitiative();
        player.RollInitiative();
        InitiativeRoll();

        // Initial setup
        UpdateskillPointsText(player.SkillPoints.ToString() + " sp");
        UpdateHealthBars();
        UpdateUltimateButtonColor();  // Set initial color for ultimate button

        // Hide combat buttons initially for enemy's turn
        DisableCombatButtons(false);

        // Start the turn system
        StartTurn();
    }

    void InitiativeRoll()
    {
        int playerInitiative = player.RollInitiative();
        int enemyInitiative = enemy.RollInitiative();
        if (playerInitiative >= enemyInitiative)
        {
            playerTurn = true;  // Player goes first if they roll higher or ties
            UpdateTurnOrder("Player goes first!");
        }
        else
        {
            playerTurn = false;  // Enemy goes first if they roll higher
            UpdateTurnOrder("Enemy goes first!");
        }

        // Delay before starting the first turn
        Invoke("StartTurn", 1.0f);
    }

    void StartTurn()
    {
        if (isCombatOver) return;  // If combat is over, do nothing
        
        if (playerTurn)
        {
            // It's the player's turn
            EnableCombatButtons(true);  // Show combat buttons for the player
            UpdateTurnOrder("Your turn!");
        }
        else
        {
            // It's the enemy's turn
            EnableCombatButtons(false);  // Hide combat buttons for the player
            UpdateTurnOrder("Enemy's turn!");

            // Delay for enemy's turn to complete
            Invoke("EnemyTurn", 1.0f); // Let the enemy attack after a short delay
        }
    }

    void PlayerAttack(string attackType)
    {
        if (!playerTurn || !player.isAlive || isCombatOver) return;

        switch (attackType)
        {
            case "Basic":
                player.BasicAttack(enemy, this);
    	        if (player.SkillPoints < player.maxSkillPoints)  // Check if skill points are 0 or less
                {
                    player.SkillPoints++;
                }
                player.GainEnergy(10); // Example energy gain after action
                break;
            case "Skill":
                if (player.SkillPoints <= 0)  // Check if skill points are 0 or less
                {
                    UpdateFeedback("Not enough Skill points!");
                    return; // Do not end the player's turn if skill points are insufficient
                }
                player.SkillAttack(enemy, this); // Perform the skill attack
                player.SkillPoints--;  // Reduce skill points by 1 after using the skill
                player.GainEnergy(20); // Example energy gain after action
                break;
            case "Ultimate":
                // Check if player has enough energy for the Ultimate attack
                if (player.energy < player.maxEnergy)
                {
                    UpdateFeedback("Not enough energy!");
                    return; // Do not end the player's turn if energy is insufficient
                }
                player.UltimateAttack(enemy, this);
                break;
        }

        // Check if enemy's health is depleted
        if (!enemy.isAlive)
        {
            UpdateHealthBars();
            EndCombat("You defeated the enemy!");
            return;
        }

        // Update energy after attack
        UpdateHealthBars();
        UpdateskillPointsText(player.SkillPoints.ToString() + " sp");
        UpdateUltimateButtonColor();  // Update ultimate button after using energy
        playerTurn = false;

        // Transition to enemy's turn
        StartTurn();
    }

    void EnemyTurn()
    {
        if (!enemy.isAlive || isCombatOver) return;  // Stop if enemy is dead or combat is over

        enemy.BasicAttack(player, this);

        // Check if the player’s health is depleted
        if (!player.isAlive)
        {
            EndCombat("You have been defeated!");
            return;
        }

        // Update health bars for both players
        UpdateHealthBars();

        playerTurn = true;  // Switch turn to player
        StartTurn();  // Call StartTurn to update the UI and enable buttons for player
    }

    // Ends the combat and displays a message
    void EndCombat(string message)
    {
        isCombatOver = true;
        UpdateFeedback(message);
        DisableCombatButtons(true);  // Hide combat buttons
        UpdateTurnOrder("Combat Over");
    }

    // Update health bars in the UI
    void UpdateHealthBars()
    {
        playerHealthBar.fillAmount = Mathf.Clamp01((float)player.health / 100);
        enemyHealthBar.fillAmount = Mathf.Clamp01((float)enemy.health / 100);
    }

    // Update feedback text for UI
    public void UpdateFeedback(string message)
    {
        feedbackText.text = message;
    }

    public void UpdateskillPointsText(string message)
    {
        skillPointsText.text = message;
    }

    public void UpdateTurnOrder(string message)
    {
        turnManager.text = message;
    }

    // Update the ultimate button color based on energy
    void UpdateUltimateButtonColor()
    {
        float fillAmount = Mathf.Clamp01((float)player.energy / player.maxEnergy);
        ultimateButtonImage.fillAmount = fillAmount;  // Set fill amount based on player's energy
    }

    // Show or hide combat buttons
    void EnableCombatButtons(bool enable)
    {
        basicAttackButton.gameObject.SetActive(enable);
        skillAttackButton.gameObject.SetActive(enable);
    }

    // Hide all combat buttons
    void DisableCombatButtons(bool disable)
    {
        basicAttackButton.gameObject.SetActive(!disable);
        skillAttackButton.gameObject.SetActive(!disable);
    }
}
