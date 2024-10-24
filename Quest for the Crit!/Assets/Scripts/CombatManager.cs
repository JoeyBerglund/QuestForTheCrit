using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public Player player; // Your player object
    public Enemy enemy; // Your enemy object

    public Text feedbackText; // UI text for feedback
    public Image playerHealthBar; // UI health bar for player
    public Image enemyHealthBar; // UI health bar for enemy
    public Button basicAttackButton; // Button for basic attack
    public Button skillAttackButton; // Button for skill attack
    public Button ultimateAttackButton; // Button for ultimate attack

    private Animator playerAnimator; // Reference to the Animator
    private bool canAction = true; // Check if the player can perform an action

    void Start()
    {
        playerAnimator = player.GetComponent<Animator>(); // Get Animator component from player
        basicAttackButton.onClick.AddListener(() => PlayerAttack("Basic"));
        skillAttackButton.onClick.AddListener(() => PlayerAttack("Skill"));
        ultimateAttackButton.onClick.AddListener(() => PlayerAttack("Ultimate"));

        UpdateFeedback("Your turn!");
        UpdateHealthBars();
    }

    void PlayerAttack(string attackType)
    {
        if (!canAction || !player.isAlive) return; // Check if the player can act and is alive

        switch (attackType)
        {
            case "Basic":
                player.BasicAttack(enemy);
                playerAnimator.SetTrigger("BasicAttackTrigger"); // Trigger Basic Attack animation
                UpdateFeedback("You attacked the enemy with Basic Attack!");
                break;
            case "Skill":
                player.SkillAttack(enemy);
                playerAnimator.SetTrigger("SkillAttackTrigger"); // Trigger Skill Attack animation
                UpdateFeedback("You used a Skill Attack!");
                break;
            case "Ultimate":
                if (player.energy >= player.maxEnergy)
                {
                    player.UltimateAttack(enemy);
                    playerAnimator.SetTrigger("UltimateAttackTrigger"); // Trigger Ultimate animation
                    UpdateFeedback("You used your Ultimate!");
                }
                else
                {
                    UpdateFeedback("Not enough energy for Ultimate!");
                    return; // Skip the turn if not enough energy
                }
                break;
        }

        canAction = false; // Prevent further actions until the current one is finished
        UpdateHealthBars();

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
            canAction = true; // Allow the player to act again
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
}
