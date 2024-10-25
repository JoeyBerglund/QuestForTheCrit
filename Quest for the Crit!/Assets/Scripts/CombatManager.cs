using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public Player player;
    public Enemy enemy;

    public Text feedbackText;
    public Image playerHealthBar;
    public Image enemyHealthBar;
    public Button basicAttackButton;
    public Button skillAttackButton;
    public Button ultimateAttackButton;
    public Image ultimateButtonImage; // Add this line

    public TurnManager turnManager;

    private bool playerTurn = true;

    void Start()
    {
        basicAttackButton.onClick.AddListener(() => PlayerAttack("Basic"));
        skillAttackButton.onClick.AddListener(() => PlayerAttack("Skill"));
        ultimateAttackButton.onClick.AddListener(() => PlayerAttack("Ultimate"));

        UpdateFeedback("Your turn!");
        UpdateHealthBars();
        UpdateUltimateButtonColor(); // Call this to set the initial color

        // Start Turn Order
        turnManager.StartTurn();
    }

    void PlayerAttack(string attackType)
    {
        if (!playerTurn || !player.isAlive) return;

        switch (attackType)
        {
            case "Basic":
                player.BasicAttack(enemy);
                UpdateFeedback($"You attacked the enemy with Basic Attack!");
                break;
            case "Skill":
                player.SkillAttack(enemy);
                UpdateFeedback($"You used a Skill Attack!");
                break;
            case "Ultimate":
                if (player.energy >= player.maxEnergy)
                {
                    player.UltimateAttack(enemy);
                    UpdateFeedback($"You used your Ultimate!");
                }
                else
                {
                    UpdateFeedback("Not enough energy for Ultimate!");
                    return; // Skip the turn if not enough energy
                }
                break;
        }

        // Update visuals
        player.GainEnergy(20); // Example energy gain after each action
        UpdateHealthBars();
        UpdateUltimateButtonColor(); // Update the button color after using energy
        playerTurn = false;

        // Delay for enemy turn
        Invoke("EnemyTurn", 1.0f);
    }

    void EnemyTurn()
    {
        if (enemy.isAlive)
        {
            enemy.BasicAttack(player);
            UpdateFeedback("Enemy attacked you!");
            UpdateHealthBars();
        }

        // Check if the player is still alive after the enemy attack
        if (player.isAlive)
        {
            playerTurn = true;
            UpdateFeedback("Your turn!");
        }
        else
        {
            UpdateFeedback("You have been defeated!");
        }
    }

    void UpdateHealthBars()
    {
        playerHealthBar.fillAmount = (float)player.health / 100;
        enemyHealthBar.fillAmount = (float)enemy.health / 100;
    }

    void UpdateFeedback(string message)
    {
        feedbackText.text = message;
    }

    void UpdateUltimateButtonColor()
    {
        float fillAmount = (float)player.energy / player.maxEnergy;
        ultimateButtonImage.fillAmount = fillAmount; // Update the fill amount based on player's energy
    }
}
