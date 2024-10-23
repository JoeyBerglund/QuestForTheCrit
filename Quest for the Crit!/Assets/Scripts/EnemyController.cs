using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public int attackPower = 10;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            // Handle enemy defeat
        }
    }
}
