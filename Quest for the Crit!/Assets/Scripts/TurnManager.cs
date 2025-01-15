using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public PlayerController player;
    public EnemyController enemy;
    private bool playerTurn = true;

    public void RollInitiative()
    {
        playerTurn = player.RollInitiative() >= enemy.RollInitiative();
    }

    public bool IsPlayerTurn() => playerTurn;

    public void EndPlayerTurn() => playerTurn = false;

    public void EndEnemyTurn() => playerTurn = true;
}
