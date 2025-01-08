using UnityEngine;

public class EnemyTargetable : MonoBehaviour
{
    public CombatManager combatManager;
    public EnemyController enemyController; // Reference to the associated EnemyController

    private void OnMouseDown()
    {
        // When the player clicks on the enemy, set it as the current target in the CombatManager
        combatManager.TargetEnemy(this.enemyController.gameObject);
    }

    public void Initialize(CombatManager manager, EnemyController controller)
    {
        combatManager = manager;
        enemyController = controller;
    }
}
