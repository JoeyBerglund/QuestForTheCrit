using UnityEngine;

public class PlayerController : Character
{
    // Player stats
    public int health = 100;
    public int attackPower = 10;
    public int energy = 0;
    public int maxEnergy = 100;
    public int armorClass = 12;
    public int maxSkillPoints = 5;
    public int SkillPoints = 5;
    public int initiativeRoll;  // Player's initiative roll

    // Override isAlive to reflect player health
    public override bool isAlive => health > 0;

    // Override TakeDamage method for the player
    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;
    }

    // Override GainEnergy for player
    public override void GainEnergy(int amount)
    {
        energy += amount;
        if (energy > maxEnergy) energy = maxEnergy;
    }

    // Roll for initiative (1d20)
    public int RollInitiative()
    {
        return Random.Range(1, 21);  // Roll a 1d20 and return the result
    }

    // Basic attack
    public int BasicAttack(EnemyController enemy, CombatManager combatManager)
    {
        int toHitRoll = RollToHit();
        int damage = 0;
        if (toHitRoll >= enemy.armorClass) // Hit!
        {
            damage = RollDamage(toHitRoll == 20); // Critical hit if it's a 20
            enemy.TakeDamage(damage); // Deal damage to the enemy
            combatManager.UpdateFeedback("Player attacks for " + damage + " damage!");
        }
        else
        {
            combatManager.UpdateFeedback("Player's attack missed!");
        }
        return damage;
    }

    public int SkillAttack(EnemyController enemy, CombatManager combatManager)
    {
        // Skill attack logic (similar to BasicAttack but may differ in the damage calculation)
        int damage = BasicAttack(enemy, combatManager);  // Placeholder for skill attack
        return damage;
    }

    public void UltimateAttack(EnemyController enemy, CombatManager combatManager)
    {
        // Ultimate attack logic (does more damage, uses more resources)
        if (energy >= maxEnergy)
        {
            energy = 0; // Reset energy
            int damage = BasicAttack(enemy, combatManager) * 2; // Example: double damage on ultimate
            enemy.TakeDamage(damage);
            combatManager.UpdateFeedback("Player uses Ultimate Attack for " + damage + " damage!");
        }
        else
        {
            combatManager.UpdateFeedback("Not enough energy for ultimate attack!");
        }
    }
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
