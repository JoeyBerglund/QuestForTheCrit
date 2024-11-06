using UnityEngine;

public class EnemyController : Character
{
    public int health = 100;
    public int attackPower = 10;
    public int armorClass = 12;
    public int initiativeRoll;  // Enemy's initiative roll


    // Override isAlive to reflect enemy health
    public override bool isAlive => health > 0;

      public int RollInitiative()
    {
        return Random.Range(1, 21);  // Roll a 1d20 and return the result
    }

    // Override TakeDamage method for the enemy
    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;
    }

    // Enemy attack
    public void BasicAttack(PlayerController player, CombatManager combatManager)
    {
        int toHitRoll = RollToHit();
        if (toHitRoll >= player.armorClass)
        {
            int damage = RollDamage(toHitRoll == 20);  // Crit if 20
            player.TakeDamage(damage);
            combatManager.UpdateFeedback(damage + " Damage from Enemy");
        }
        else
        {
            combatManager.UpdateFeedback("Enemy missed!");
        }
    }

    // Helper to roll a 1d20 for attack check
    private int RollToHit()
    {
        return Random.Range(1, 21); // Rolls between 1 and 20
    }

    // Helper to roll damage (1d12, double damage on crit)
    private int RollDamage(bool isCrit)
    {
        int damage = Random.Range(1, 13); // Roll 1d12
        if (isCrit)
        {
            damage += Random.Range(1, 13);  // Additional damage for crit
            Debug.Log("Enemy Critical hit!");
        }
        return damage;
    }
}
