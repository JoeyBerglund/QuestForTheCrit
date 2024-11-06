using UnityEngine;

public class EnemyTargetable : MonoBehaviour
{
    public CombatManager combatManager;

    private void OnMouseDown()
    {
        // When the player clicks on the enemy, set it as the current target in the CombatManager
        combatManager.TargetEnemy(this.gameObject);
    }
}
