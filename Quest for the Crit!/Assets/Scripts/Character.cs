using UnityEngine;

public class Character : MonoBehaviour
{
    public int health = 100;
    public int attackPower = 10;
    public int speed = 60;
    public int energy = 0;

    public int maxEnergy = 100; // Energy required to use the ultimate

    public bool isAlive => health > 0;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
        }
    }

    // Basic Attack
    public void BasicAttack(Character target)
    {
        target.TakeDamage(attackPower);
    }

    // Skill: A more powerful attack that may cost energy or have cooldown
    public void SkillAttack(Character target)
    {
        target.TakeDamage(attackPower * 2); // Example: stronger than basic attack
    }

    // Ultimate: Available when energy is full
    public void UltimateAttack(Character target)
    {
        if (energy >= maxEnergy)
        {
            target.TakeDamage(attackPower * 4); // Example: much stronger attack
            energy = 0; // Reset energy after ultimate
        }
    }

    // Gain energy during the battle
    public void GainEnergy(int amount)
    {
        energy += amount;
        if (energy > maxEnergy) energy = maxEnergy;
    }
}
