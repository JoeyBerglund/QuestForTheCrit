using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public PlayerController player;  // Reference to PlayerController
    public EnemyController[] enemies; // Reference to all enemies in the scene
    public EnemyController enemy;    // Reference to EnemyController

    // UI elements
    public Text feedbackText;
    public Image playerHealthBar;
    public Image enemyHealthBar;
    public Button basicAttackButton;
    public Button skillAttackButton;
    public Button ultimateAttackButton;
    public Image ultimateButtonImage;
    public Text skillPointsText;
    private Material defaultMaterial; // To store the original material
    public Material highlightMaterial;
    public Text turnManager;  // Reference to turn manager

    public bool playerTurn = true;  // Track whose turn it is
    private bool isCombatOver = false;  // Track if combat is over
    private GameObject targetedEnemy; // Variable to store the targeted enemy
    void Start()
    {
        // Button listeners
        basicAttackButton.onClick.AddListener(() => PlayerAttack("Basic"));
        skillAttackButton.onClick.AddListener(() => PlayerAttack("Skill"));
        ultimateAttackButton.onClick.AddListener(() => PlayerAttack("Ultimate"));

       
        
        // Initial setup
        UpdateskillPointsText(player.SkillPoints.ToString() + " sp");
        UpdateHealthBars();
        ultimateButtonImage.fillAmount = 0;
        UpdateUltimateButtonColor();  // Set initial color for ultimate button

        // Roll for initiative at the start
        enemy = FindObjectOfType<EnemyController>();  // Find the enemy in the scene
        enemy.RollInitiative();
        player.RollInitiative();
        InitiativeRoll();
        // Start the turn system
        StartTurn();
    }

    void InitiativeRoll()
    {
        if (player == null || enemy == null)
        {
            Debug.LogError("Player or Enemy is not assigned in CombatManager.");
            return;
        }

        int playerInitiative = 0; // player.RollInitiative();  // Player's initiative roll
        int enemyInitiative = enemy.RollInitiative();    // Enemy's initiative roll
        
        if (playerInitiative >= enemyInitiative)
        {
            playerTurn = true;  // Player goes first if they roll higher or tie
            UpdateTurnOrder("Player goes first! (" + playerInitiative + " vs " + enemyInitiative + ")");
        }
        else
        {
            playerTurn = false;  // Enemy goes first if they roll higher
            UpdateTurnOrder("Enemy goes first! (" + playerInitiative + " vs " + enemyInitiative + ")");
        }

        // Delay before starting the first turn
        Invoke("StartTurn", 1.0f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isCombatOver)  
        {
            TryTargetEnemy();
        }
    }

    void TryTargetEnemy()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  

        if (Physics.Raycast(ray, out hit))
        {
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null && enemy.isAlive)
            {
                TargetEnemy(enemy.gameObject);
            }
        }
    }

    public void TargetEnemy(GameObject enemyObject)
    {
        if (targetedEnemy != null && targetedEnemy != enemyObject)
        {
            // Reset the material of the previous target to its default
            ResetPreviousTarget();
        }

        targetedEnemy = enemyObject;

        // Highlight the new target
        HighlightTarget(targetedEnemy);

        // Update feedback
        UpdateFeedback("Targeted enemy: " + enemyObject.name); 
        Debug.Log("Targeted: " + enemyObject.name);
    }

    void HighlightTarget(GameObject enemyObject)
    {
        Renderer enemyRenderer = enemyObject.GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            defaultMaterial = enemyRenderer.material;  // Store the original material
            enemyRenderer.material = highlightMaterial; // Change to highlight material
        }
    }

    void ResetPreviousTarget()
    {
        if (targetedEnemy != null)
        {
            Renderer previousRenderer = targetedEnemy.GetComponent<Renderer>();
            if (previousRenderer != null)
            {
                previousRenderer.material = defaultMaterial; // Reset the material back to original
            }
        }
    }

    void PlayerAttack(string attackType)
    {
        if (targetedEnemy == null)
        {
            UpdateFeedback("No enemy targeted!");
            return;
        }

        if (!playerTurn || !player.isAlive || isCombatOver) return;

        // Attack the targeted enemy instead of the default one
        EnemyController targetedEnemyController = targetedEnemy.GetComponent<EnemyController>();

        if (targetedEnemyController != null)
        {
            switch (attackType)
            {
                case "Basic":
                player.BasicAttack(enemy, this);
    	        if (player.SkillPoints < player.maxSkillPoints)  // Check if skill points are 0 or less
                {
                    player.SkillPoints++;
                }
                player.GainEnergy(10); // Example energy gain after action
                break;
            case "Skill":
                if (player.SkillPoints <= 0)  // Check if skill points are 0 or less
                {
                    UpdateFeedback("Not enough Skill points!");
                    return; // Do not end the player's turn if skill points are insufficient
                }
                player.SkillAttack(enemy, this); // Perform the skill attack
                player.SkillPoints--;  // Reduce skill points by 1 after using the skill
                player.GainEnergy(20); // Example energy gain after action
                break;
            case "Ultimate":
                // Check if player has enough energy for the Ultimate attack
                if (player.energy < player.maxEnergy)
                {
                    UpdateFeedback("Not enough energy!");
                    return; // Do not end the player's turn if energy is insufficient
                }
                player.UltimateAttack(enemy, this);
                break;
            }

            // Check if the targeted enemy is dead and handle the end of combat if necessary
            if (!targetedEnemyController.isAlive)
            {
                UpdateHealthBars();
                EndCombat("You defeated the enemy!");
                return;
            }

            UpdateHealthBars();
            UpdateskillPointsText(player.SkillPoints.ToString() + " sp");
            UpdateUltimateButtonColor();
            playerTurn = false;
            StartTurn();
        }
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

            // Delay for enemy's turn to complete
            Invoke("EnemyTurn", 1.0f);  // Let the enemy attack after a short delay
        }
    }

    void EnemyTurn()
    {
        if (!enemy.isAlive || isCombatOver) return;

        // Enemy always targets the player
        enemy.BasicAttack(player, this);

        // Check if the player's health is depleted
        if (!player.isAlive)
        {
            EndCombat("You have been defeated!");
            return;
        }

        // Update health bars for both players
        UpdateHealthBars();
        playerTurn = true;
        StartTurn();
    }

    public void EndCombat(string message)
    {
        isCombatOver = true;
        UpdateFeedback(message);
        DisableCombatButtons(true);  // Hide combat buttons
        UpdateTurnOrder("Combat Over");
    }

    void UpdateHealthBars()
    {
        playerHealthBar.fillAmount = Mathf.Clamp01((float)player.health / 100);
        enemyHealthBar.fillAmount = Mathf.Clamp01((float)targetedEnemy.GetComponent<EnemyController>().health / 100);
    }

    public void UpdateFeedback(string message)
    {
        feedbackText.text = message;
    }

    public void UpdateskillPointsText(string message)
    {
        skillPointsText.text = message;
    }

    public void UpdateTurnOrder(string message)
    {
        turnManager.text = message;
    }

    void UpdateUltimateButtonColor()
    {
        float fillAmount = Mathf.Clamp01((float)player.energy / player.maxEnergy);
        ultimateButtonImage.fillAmount = fillAmount;
    }

    void EnableCombatButtons(bool enable)
    {
        basicAttackButton.gameObject.SetActive(enable);
        skillAttackButton.gameObject.SetActive(enable);
    }

    void DisableCombatButtons(bool disable)
    {
        basicAttackButton.gameObject.SetActive(!disable);
        skillAttackButton.gameObject.SetActive(!disable);
    }
}
