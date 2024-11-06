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
        enemy.TakeDamage(damage);
        combatManager.UpdateFeedback(damage + " Damage");
    }
    else
    {
        combatManager.UpdateFeedback("You missed!");
    }
    return damage; // Return the damage dealt
}

public int SkillAttack(EnemyController enemy, CombatManager combatManager)
{
    int toHitRoll = RollToHit();
    int damage = 0;
    if (toHitRoll >= enemy.armorClass) // Hit!
    {
        damage = RollDamage(toHitRoll == 20); // Critical hit if it's a 20
        damage = damage * 2; // Double damage for skill attack
        enemy.TakeDamage(damage);
        combatManager.UpdateFeedback(damage + " Damage");
    }
    else
    {
        combatManager.UpdateFeedback("You missed!");
    }
    return damage; // Return the damage dealt
}

public int UltimateAttack(EnemyController enemy, CombatManager combatManager)
{
    if (energy >= maxEnergy)
    {
        // Increase the chance of a critical hit by 20%
        int toHitRoll = RollToHit();

        // If the roll is 16 or higher (20% increased chance for crit)
        bool isCrit = toHitRoll >= 16;

        // Roll damage with the crit chance
        int damage = RollDamage(isCrit); // isCrit will be true if the roll was 16 or higher
        enemy.TakeDamage((damage * 2) + 15);  // Extra damage for ultimate
        energy = 0;  // Reset energy after ultimate
        int totalDamage = damage + 15;
        combatManager.UpdateFeedback(totalDamage + " Damage");
        return damage + 15; // Return the total damage dealt (including extra from ultimate)
    }
    else
    {
        combatManager.UpdateFeedback("Not enough energy!");
        return 0;  // Return 0 if ultimate couldn't be used
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
        if (isCrit) // If critical hit, roll again and add the result
        {
            damage += Random.Range(1, 13); // Roll another 1d12 for crit damage
            Debug.Log("Critical hit!");
        }
        return damage;
    }
}
