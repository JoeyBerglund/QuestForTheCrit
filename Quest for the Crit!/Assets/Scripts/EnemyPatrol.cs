using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;          // Movement speed of the enemy
    public float patrolDistance = 20f; // Distance the enemy moves in one direction from the start position

    private Vector3 leftPoint;       // The leftmost patrol point
    private Vector3 rightPoint;      // The rightmost patrol point
    private Vector3 targetPoint;     // The current target point
    private bool movingRight = true; // Whether the enemy is moving right

    void Start()
    {
        // Set up the patrol points
        leftPoint = transform.position - new Vector3(patrolDistance, 0f, 0f);
        rightPoint = transform.position + new Vector3(patrolDistance, 0f, 0f);

        // Start moving towards the right point first
        targetPoint = rightPoint;
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        // Move towards the target point
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        // Check if the enemy has reached the target point
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            // Switch the target point
            if (movingRight)
            {
                targetPoint = leftPoint;
                movingRight = false;
            }
            else
            {
                targetPoint = rightPoint;
                movingRight = true;
            }
        }
    }
}
