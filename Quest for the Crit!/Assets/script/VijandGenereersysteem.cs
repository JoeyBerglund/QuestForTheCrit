using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : MonoBehaviour
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string[] Locations { get; set; }
    public GameObject Prefab { get; set; } // Prefab is now a GameObject instead of a string
}

public class VijandGenereersysteem : MonoBehaviour
{
    // List to hold enemies
    public List<Enemy> enemies;
    public GameObject skeletonPrefab;
    public GameObject slimePrefab;

    // Start is called before the first frame update
    void Start()
    {
        var selectedEnemy = GetEnemy();
        Debug.Log("selectedEnemy");
        Debug.Log($"Id: {selectedEnemy.Id}, Name: {selectedEnemy.Name}, Locations: {string.Join(", ", selectedEnemy.Locations)}, Prefab: {selectedEnemy.Prefab}");
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Method to get an enemy based on the current location
    public Enemy GetEnemy()
    {
        var currentLocation = GetLocation();
        var enemies = AllEnemys();
        // Filter enemies that exist in the current location
        var availableEnemies = enemies.Where(enemy => enemy.Locations.Contains(currentLocation)).ToList();

        // Log available enemies
        Debug.Log("availableEnemies");
        foreach (var enemy in availableEnemies)
        {
            Debug.Log($"Id: {enemy.Id}, Name: {enemy.Name}, Locations: {string.Join(", ", enemy.Locations)}, Prefab: {enemy.Prefab}");
        }

        // Return a random enemy from available ones (or null if no enemies found)
        if (availableEnemies.Count > 0)
        {
            return availableEnemies[UnityEngine.Random.Range(0, availableEnemies.Count)];
        }
        else
        {
            Debug.Log("No enemies available at the current location.");
            return null;
        }
    }

    // Example method to get the current location
    public string GetLocation()
    {
        return "Location1";
    }
    public List<Enemy> AllEnemys()
    {
        var enemies = new List<Enemy>
        {
            new Enemy { Id = 1, Name = "Skeleton", Locations = new string[] { "Location1", "Location2" }, Prefab = skeletonPrefab },
            new Enemy { Id = 2, Name = "Slime", Locations = new string[] { "Location3", "Location1" }, Prefab = slimePrefab }
        };

        foreach (var enemy in enemies)
        {
            if (enemy.Prefab == null)
            {
                Debug.LogWarning($"Prefab for Enemy '{enemy.Name}' not found. Check the path in Resources.");
            }
            else
            {
                Debug.Log($"Loaded Enemy - Id: {enemy.Id}, Name: {enemy.Name}, Prefab: {enemy.Prefab.name}");
            }
        }

        return enemies;
    }

}
