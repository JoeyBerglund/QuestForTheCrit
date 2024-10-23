using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public Player player;
    public Enemy enemy;

    public Text feedbackText;
    public Image playerHealthBar;
    public Image enemyHealthBar;
    public Button attackButton;
    public Button defendButton;

    private bool playerTurn = true;

    void Start()
    {
        attackButton.onClick.AddListener(PlayerAttack);
        defendButton.onClick.AddListener(PlayerDefend);
        UpdateFeedback("Your turn!");
        UpdateHealthBars();
    }

    void PlayerAttack()
    {
        if (!playerTurn) return;
        enemy.TakeDamage(player.attackPower);
        UpdateHealthBars();
        UpdateFeedback($"You attacked the enemy for {player.attackPower} damage!");
        playerTurn = false;
        Invoke("EnemyTurn", 1.0f); // Delay for enemy turn
    }

    void PlayerDefend()
    {
        if (!playerTurn) return;
        UpdateFeedback("You defended!");
        playerTurn = false;
        Invoke("EnemyTurn", 1.0f);
    }

    void EnemyTurn()
    {
        player.TakeDamage(enemy.attackPower);
        UpdateHealthBars();
        UpdateFeedback($"Enemy attacked you for {enemy.attackPower} damage!");
        playerTurn = true;

        // Check for win/lose conditions
        if (player.health <= 0)
        {
            UpdateFeedback("You have been defeated!");
        }
        else if (enemy.health <= 0)
        {
            UpdateFeedback("You defeated the enemy!");
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
