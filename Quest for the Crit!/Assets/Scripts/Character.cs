using UnityEngine;

public class Character : MonoBehaviour
{
    // Shared functionality for all characters

    // Method for taking damage (to be overridden in PlayerController and EnemyController)
    public virtual void TakeDamage(int damage)
    {
        // This method will be overridden in derived classes
    }

    // Method for gaining energy (to be overridden in PlayerController and EnemyController)
    public virtual void GainEnergy(int amount)
    {
        // This method will be overridden in derived classes
    }

    // Method for checking if the character is alive (to be overridden in PlayerController and EnemyController)
    public virtual bool isAlive => false;
}
