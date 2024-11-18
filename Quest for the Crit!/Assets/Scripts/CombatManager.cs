using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public PlayerController player;
    public EnemyController[] enemyPrefabs;
    public GameObject healthBarPrefab;
    public Canvas healthBarCanvas; // Add the health bar canvas here
    public List<EnemyController> activeEnemies = new List<EnemyController>();
    public Text feedbackText, skillPointsText, turnManagerText;
    public Image playerHealthBar, ultimateButtonImage;
    public Button basicAttackButton, skillAttackButton, ultimateAttackButton;
    public GameObject targetCirclePrefab;

    private bool playerTurn = true;
    private bool isCombatOver = false;
    private GameObject targetedEnemy;
    private GameObject activeTargetCircle;
    private int currentEnemyIndex = 0;

    void Start()
    {
        basicAttackButton.onClick.AddListener(() => PlayerAttack("Basic"));
        skillAttackButton.onClick.AddListener(() => PlayerAttack("Skill"));
        ultimateAttackButton.onClick.AddListener(() => PlayerAttack("Ultimate"));

        UpdateSkillPointsText();
        UpdateHealthBars();
        UpdateUltimateButtonFill();

        RollInitiative();
        SpawnRandomEnemies();
    }

    void SpawnRandomEnemies()
    {
        Vector3 centralPosition = new Vector3(-20.48f, 0, -28.02f);
        float xOffset = 0.6f;
        Vector3[] spawnPositions = { centralPosition + new Vector3(0, 0, -xOffset), centralPosition, centralPosition + new Vector3(0, 0, xOffset) };

        foreach (var spawnPosition in spawnPositions)
        {
            EnemyController randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Quaternion rotation = Quaternion.Euler(0, 90, 0);

            EnemyController enemyInstance = Instantiate(randomEnemyPrefab, spawnPosition, rotation);
            activeEnemies.Add(enemyInstance);

            // Instantiate health bar as a child of the healthBarCanvas in world space
            GameObject healthBar = Instantiate(healthBarPrefab, healthBarCanvas.transform);
            healthBar.transform.position = spawnPosition + new Vector3(0, 1.5f, 0);

            enemyInstance.SetHealthBar(healthBar.GetComponent<Image>());

            // Set the canvas in the enemy instance
            enemyInstance.InitializeHealthBarPosition(healthBarCanvas);

            EnemyTargetable targetable = enemyInstance.GetComponent<EnemyTargetable>();
            if (targetable != null)
            {
                targetable.Initialize(this, enemyInstance);
            }
        }
    }

    void RollInitiative()
    {
        playerTurn = player.RollInitiative() >= activeEnemies[0].RollInitiative();
        UpdateTurnOrder(playerTurn ? "Player goes first!" : "Enemies go first!");
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
        
        // Instantiate the target circle at the correct position and with a 90-degree rotation around the Y-axis
        activeTargetCircle = Instantiate(targetCirclePrefab, enemyObject.transform.position + new Vector3(0f, 1.5f, 0), Quaternion.Euler(0, 90, 0));
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
                    else
                    {
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
                    else
                    {
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
            UpdateTurnOrder("Enemies' turn!");
            currentEnemyIndex = 0;  // Reset to the first enemy
            Invoke(nameof(EnemyTurn), 1.0f);
        }
    }

    void EnemyTurn()
    {
        if (isCombatOver || currentEnemyIndex >= activeEnemies.Count) return;

        EnemyController currentEnemy = activeEnemies[currentEnemyIndex];

        if (currentEnemy.isAlive)
        {
            currentEnemy.BasicAttack(player, this);
        }

        UpdateHealthBars();

        if (!player.isAlive)
        {
            EndCombat("You have been defeated!");
            return;
        }

        currentEnemyIndex++;
        if (currentEnemyIndex < activeEnemies.Count)
        {
            Invoke(nameof(EnemyTurn), 1.0f);  // Move to the next enemy's turn
        }
        else
        {
            playerTurn = true;
            StartTurn();
        }
    }

    public void EndCombat(string message)
    {
        isCombatOver = true;
        UpdateFeedback(message);
        EnableCombatButtons(false);
        UpdateTurnOrder("Combat Over");
        ClearPreviousTarget();
    }

    // Add method to update the player's health bar
    void UpdateHealthBars()
    {
        // Update the health bar for the player
        playerHealthBar.fillAmount = Mathf.Clamp01((float)player.health / player.maxHealth);

        // Update the health bars for each enemy
        foreach (var enemy in activeEnemies)
        {
            enemy.UpdateHealthBar();
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
