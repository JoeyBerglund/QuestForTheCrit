using UnityEngine;

public class Character : MonoBehaviour
{
    public virtual void TakeDamage(int damage) { }

    public virtual void GainEnergy(int amount) { }

    public virtual bool isAlive => false;
}
