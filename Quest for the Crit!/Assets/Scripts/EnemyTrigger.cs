using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTrigger : MonoBehaviour
{
    public string CombatScene; // Name of the scene to load when the player enters the trigger

    private Collider enemyCollider; // Reference to the enemy's collider

    void Start()
    {
        // Get the collider attached to this GameObject
        enemyCollider = GetComponent<Collider>();

        if (enemyCollider == null)
        {
            Debug.LogError("No collider found on the enemy object!");
        }
    }

    void Update()
    {
        // Check for collision with the player manually
        Collider[] hits = Physics.OverlapBox(
            enemyCollider.bounds.center,
            enemyCollider.bounds.extents / 2,
            Quaternion.identity
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Player has entered the trigger area
                Debug.Log("Player entered the trigger!");
                SceneManager.LoadScene(CombatScene);
                break; // No need to check further once the player is found
            }
        }
    }
}
