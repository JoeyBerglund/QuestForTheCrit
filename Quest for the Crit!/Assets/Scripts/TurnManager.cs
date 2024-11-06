using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public PlayerController player;
    public EnemyController enemy;

    private bool playerTurn = true;

    public void RollInitiative()
    {
        player.initiativeRoll = player.RollInitiative();
        enemy.initiativeRoll = enemy.RollInitiative();

        if (player.initiativeRoll >= enemy.initiativeRoll)
        {
            playerTurn = true;  // Player goes first
        }
        else
        {
            playerTurn = false;  // Enemy goes first
        }
    }

    public bool IsPlayerTurn()
    {
        return playerTurn;
    }

    public void EndPlayerTurn()
    {
        playerTurn = false;
    }

    public void EndEnemyTurn()
    {
        playerTurn = true;
    }
}
