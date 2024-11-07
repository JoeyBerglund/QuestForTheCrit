using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public PlayerController player;
    public EnemyController[] enemyPrefabs;  // Array of enemy prefabs to choose from
    public GameObject healthBarPrefab;      // Health bar prefab (Image type)
    public List<EnemyController> activeEnemies = new List<EnemyController>(); // List of active enemies
    public List<GameObject> activeHealthBars = new List<GameObject>(); // List of health bar instances
    public Text feedbackText, skillPointsText, turnManagerText;
    public Image playerHealthBar, ultimateButtonImage;
    public Button basicAttackButton, skillAttackButton, ultimateAttackButton;
    public GameObject targetCirclePrefab;

    private bool playerTurn = true;
    private bool isCombatOver = false;
    private GameObject targetedEnemy;
    private GameObject activeTargetCircle;

    void Start()
    {
        // Button listeners for clicks
        basicAttackButton.onClick.AddListener(() => PlayerAttack("Basic"));
        skillAttackButton.onClick.AddListener(() => PlayerAttack("Skill"));
        ultimateAttackButton.onClick.AddListener(() => PlayerAttack("Ultimate"));

        // Initial setup
        UpdateSkillPointsText();
        UpdateHealthBars();
        UpdateUltimateButtonFill();

        // Roll initiative and spawn enemies
        RollInitiative();
        SpawnRandomEnemies();
    }

    // Function to spawn 3 random enemies at random positions
    void SpawnRandomEnemies()
    {
        // Starting position for the first enemy
        Vector3 startPosition = new Vector3(-20.48f, 1f, -28.02f);

        // Offset values to space enemies out (e.g., spacing 5 units apart)
        float xOffset = 5f;
        float zOffset = 5f;

        for (int i = 0; i < 3; i++)
        {
            // Calculate the spawn position for each enemy, based on the index
            Vector3 spawnPosition = startPosition + new Vector3(xOffset * i, 0, zOffset * i);

            // Choose a random enemy prefab
            EnemyController randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Instantiate the enemy at the calculated position
            EnemyController enemyInstance = Instantiate(randomEnemyPrefab, spawnPosition, Quaternion.identity);

            // Add the newly spawned enemy to the activeEnemies list
            activeEnemies.Add(enemyInstance);

            // Instantiate and position a health bar above the enemy
            GameObject healthBar = Instantiate(healthBarPrefab, spawnPosition + new Vector3(0, 1.5f, 0), Quaternion.identity);
            activeHealthBars.Add(healthBar);

            // Assign the health bar to the enemy instance
            enemyInstance.SetHealthBar(healthBar.GetComponent<Image>());
        }
    }

    void RollInitiative()
    {
        playerTurn = player.RollInitiative() >= activeEnemies[0].RollInitiative();
        UpdateTurnOrder(playerTurn ? "Player goes first!" : "Enemy goes first!");
        Invoke(nameof(StartTurn), 1.0f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isCombatOver) TryTargetEnemy();
        
        // Key press detection for attacks
        if (!isCombatOver && playerTurn)
        {
            if (Input.GetKeyDown(KeyCode.Q)) // Q for Basic Attack
            {
                PlayerAttack("Basic");
            }
            else if (Input.GetKeyDown(KeyCode.E)) // E for Skill Attack
            {
                PlayerAttack("Skill");
            }
            else if (Input.GetKeyDown(KeyCode.Space)) // Spacebar for Ultimate Attack
            {
                PlayerAttack("Ultimate");
            }
        }
    }

    void TryTargetEnemy()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            EnemyController hitEnemy = hit.collider.GetComponent<EnemyController>();
            if (hitEnemy != null && hitEnemy.isAlive) TargetEnemy(hitEnemy.gameObject);
        }
    }

    public void TargetEnemy(GameObject enemyObject)
    {
        if (targetedEnemy != enemyObject)
        {
            ClearPreviousTarget();
            targetedEnemy = enemyObject;
            PlaceTargetCircle(targetedEnemy);
            UpdateFeedback("Targeted enemy: " + enemyObject.name);
        }
    }

    void PlaceTargetCircle(GameObject enemyObject)
    {
        if (activeTargetCircle != null) Destroy(activeTargetCircle);
        activeTargetCircle = Instantiate(targetCirclePrefab, enemyObject.transform.position + new Vector3(0.4f, 0.8f, 0), Quaternion.Euler(0, 90, 0));
    }

    void ClearPreviousTarget()
    {
        if (activeTargetCircle != null) Destroy(activeTargetCircle);
        targetedEnemy = null;
    }

    void PlayerAttack(string attackType)
    {
        if (targetedEnemy == null || !playerTurn || isCombatOver || !player.isAlive) return;

        EnemyController targetedEnemyController = targetedEnemy.GetComponent<EnemyController>();

        if (targetedEnemyController != null)
        {
            switch (attackType)
            {
                case "Basic":
                    player.BasicAttack(targetedEnemyController, this);
                    player.SkillPoints = Mathf.Min(player.SkillPoints + 1, player.maxSkillPoints);
                    player.GainEnergy(10);
                    break;
                case "Skill":
                    if (player.SkillPoints > 0)
                    {
                        player.SkillAttack(targetedEnemyController, this);
                        player.SkillPoints--;
                        player.GainEnergy(20);
                    }
                    else{
                        UpdateFeedback("Not enough Skill points!");
                        return;
                    } 
                    break;
                case "Ultimate":
                    if (player.energy >= player.maxEnergy)
                    {
                        player.UltimateAttack(targetedEnemyController, this);
                        player.energy = 0;
                    }
                    else{ 
                        UpdateFeedback("Not enough energy!");
                        return;
                    }
                    break;
            }

            if (!targetedEnemyController.isAlive) EndCombat("You defeated the enemy!");

            UpdateHealthBars();
            UpdateSkillPointsText();
            UpdateUltimateButtonFill();
            playerTurn = false;
            StartTurn();
        }

        ClearPreviousTarget();
    }

    public void StartTurn()
    {
        if (isCombatOver) return;
        if (playerTurn)
        {
            EnableCombatButtons(true);
            UpdateTurnOrder("Your turn!");
        }
        else
        {
            EnableCombatButtons(false);
            UpdateTurnOrder("Enemy's turn!");
            Invoke(nameof(EnemyTurn), 1.0f);
        }
    }

    void EnemyTurn()
    {
        if (activeEnemies.Count == 0 || isCombatOver) return;

        EnemyController currentEnemy = activeEnemies[0];
        currentEnemy.BasicAttack(player, this);

        if (!player.isAlive) EndCombat("You have been defeated!");

        UpdateHealthBars();
        playerTurn = true;
        StartTurn();
    }

    public void EndCombat(string message)
    {
        isCombatOver = true;
        UpdateFeedback(message);
        EnableCombatButtons(false);
        UpdateTurnOrder("Combat Over");
        ClearPreviousTarget();
    }

    void UpdateHealthBars()
    {
        foreach (var enemy in activeEnemies)
        {
            enemy.UpdateHealthBar();  // Update health bar for each enemy
        }
    }

    public void UpdateFeedback(string message) => feedbackText.text = message;

    public void UpdateSkillPointsText() => skillPointsText.text = $"{player.SkillPoints} SP";

    public void UpdateTurnOrder(string message) => turnManagerText.text = message;

    void UpdateUltimateButtonFill() => ultimateButtonImage.fillAmount = Mathf.Clamp01((float)player.energy / player.maxEnergy);

    void EnableCombatButtons(bool enable)
    {
        basicAttackButton.interactable = enable;
        skillAttackButton.interactable = enable;
        ultimateAttackButton.interactable = enable;
    }
}
