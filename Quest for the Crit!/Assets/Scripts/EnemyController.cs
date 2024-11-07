using UnityEngine;
using UnityEngine.UI;  // For the Image component

public class EnemyController : Character
{
    public int health = 100, maxHealth = 100, attackPower = 10, armorClass = 12;
    public Image healthBarImage; // Reference to the health bar image

    public override bool isAlive => health > 0;

    public override void TakeDamage(int damage)
    {
        health = Mathf.Max(health - damage, 0);
        UpdateHealthBar();
    }

    public void SetHealthBar(Image healthBar) 
    {
        healthBarImage = healthBar;
        UpdateHealthBar(); // Update health bar initially when assigned
    }

    // Update the health bar to reflect the current health
    public void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = Mathf.Clamp01((float)health / maxHealth);
        }
    }

    public void BasicAttack(PlayerController player, CombatManager combatManager)
    {
        int toHitRoll = Random.Range(1, 21);
        if (toHitRoll >= player.armorClass)
        {
            int damage = Random.Range(1, 11);
            player.TakeDamage(damage);
            combatManager.UpdateFeedback("Enemy attacks for " + damage + " damage!");
        }
        else
        {
            combatManager.UpdateFeedback("Enemy's attack missed!");
        }
    }

    public int RollInitiative() => Random.Range(1, 21);
}
