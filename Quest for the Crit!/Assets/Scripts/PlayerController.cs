using UnityEngine;

public class PlayerController : Character
{
    public int hitbonus = 5, health = 100, maxHealth = 100, attackPower = 10, energy = 0, maxEnergy = 100, armorClass = 12, maxSkillPoints = 5, SkillPoints = 5;

    public override bool isAlive => health > 0;

    public override void TakeDamage(int damage) => health = Mathf.Max(health - damage, 0);

    public override void GainEnergy(int amount) => energy = Mathf.Min(energy + amount, maxEnergy);

    public int BasicAttack(EnemyController enemy, CombatManager combatManager)
    {
        int toHitRoll = RollToHit();
        if (toHitRoll >= enemy.armorClass)
        {
            int damage = RollDamage(toHitRoll == 20);
            enemy.TakeDamage(damage);
            combatManager.UpdateFeedback("Player attacks for " + damage + " damage!");
            return damage;
        }
        combatManager.UpdateFeedback("Player's attack missed!");
        return 0;
    }

    public int SkillAttack(EnemyController enemy, CombatManager combatManager)
    {
        int toHitRoll = RollToHit();  // Roll to hit for the skill attack
        if (toHitRoll >= enemy.armorClass)  // Check if it hits
        {
            int damage = RollDamage(toHitRoll == 20);  // Roll damage, checking for crit (if toHitRoll == 20)
            damage *= 2;  // Double the damage for the skill attack
            enemy.TakeDamage(damage);
            combatManager.UpdateFeedback("Player uses Skill Attack for " + damage + " damage!");
            return damage;
        }
        else
        {
            combatManager.UpdateFeedback("Skill Attack missed!");
            return 0;  // If the attack misses, return 0
        }
    }

    public void UltimateAttack(EnemyController enemy, CombatManager combatManager)
    {
        if (energy >= maxEnergy)
        {
            int damage = Random.Range(5, 11) * 2 + 15;
            enemy.TakeDamage(damage);
            combatManager.UpdateFeedback("Player uses Ultimate Attack for " + damage + " damage!");
            energy = 0;
        }
        else combatManager.UpdateFeedback("Not enough energy for ultimate attack!");
    }

    private int RollToHit() => Random.Range(1, 21) + hitbonus;

    private int RollDamage(bool isCrit) => Random.Range(1, 13) + (isCrit ? Random.Range(1, 13) : 0);

    public int RollInitiative() => Random.Range(1, 21);
}
