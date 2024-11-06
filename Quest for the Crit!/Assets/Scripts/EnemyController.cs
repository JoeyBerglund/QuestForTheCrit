using UnityEngine;

public class EnemyController : Character
{
    public int health = 100;
    public int attackPower = 10;
    public int armorClass = 12;
    public int initiativeRoll;  // Player's initiative roll

    // Override isAlive to reflect enemy health
    public override bool isAlive => health > 0;

    // Override TakeDamage method for the enemy
    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;
    }

    public void BasicAttack(PlayerController player, CombatManager combatManager)
    {
        int toHitRoll = Random.Range(1, 21);  // Roll to hit (1d20)
        int damage = 0;

        if (toHitRoll >= player.armorClass)  // Hit!
        {
            damage = Random.Range(1, 11);  // Random damage from 1 to 10
            player.TakeDamage(damage);     // Deal damage to the player
            combatManager.UpdateFeedback("Enemy attacks for " + damage + " damage!");
        }
        else
        {
            combatManager.UpdateFeedback("Enemy's attack missed!");
        }
    }

    public int RollInitiative()
    {
        return Random.Range(1, 21);  // Roll for initiative (1d20)
    }
}
