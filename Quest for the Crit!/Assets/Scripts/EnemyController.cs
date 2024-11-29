using UnityEngine;
using UnityEngine.UI;

public class EnemyController : Character
{
    public int health = 100, maxHealth = 100, attackPower = 10, armorClass = 12;
    private Image healthBarImage; // Reference to the health bar's fill image
    private Canvas healthBarCanvas; // Reference to the health bar canvas
    private Animator animator; // Reference to the Animator component

    // Check if the enemy is alive
    public override bool isAlive => health > 0;


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError($"Animator not found on {gameObject.name}");
        }
    }
    // This method is called when the enemy takes damage
    public override void TakeDamage(int damage)
    {
        if (health > 0)
        {
            animator.SetTrigger("TakeDamage"); // Play the "TakeDamage" animation
            health = Mathf.Max(health - damage, 0); // Reduce health and clamp to zero
            UpdateHealthBar(); // Update the health bar after taking damage
        }

        if (health == 0)
        {
            animator.SetTrigger("Die"); // Play the "Die" animation

            // Destroy the GameObject after the death animation plays
            Destroy(healthBarImage);
            Destroy(gameObject, 4.0f); // Delay destruction to allow animation to finish

            // No need to call EndCombat here — it should be handled in CombatManager after the enemy's turn
        }
    }



    // Set the health bar fill image and update the health bar
    public void SetHealthBar(Image healthBarFill)
    {
        healthBarImage = healthBarFill; // Set the reference to the fill Image
        Debug.Log($"Health bar set for {gameObject.name}"); // Debug to confirm assignment
        UpdateHealthBar(); // Ensure health bar is updated immediately
    }

    // Initialize the health bar position by setting the canvas reference
    public void InitializeHealthBarPosition(Canvas canvas)
    {
        healthBarCanvas = canvas; // Set the reference to the health bar canvas
    }

    // Update the health bar's fill amount based on current health
    public void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = Mathf.Clamp01((float)health / maxHealth);
        }
    }

    // Update the health bar position to follow the enemy in world space
    private void UpdateHealthBarPosition()
    {
        if (healthBarImage == null || healthBarCanvas == null) return;

        // Convert the world position of the enemy to the canvas' local position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.5f, 0));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            healthBarCanvas.transform as RectTransform,
            screenPosition,
            healthBarCanvas.worldCamera,
            out Vector2 localPoint
        );

        // Set the anchored position of the health bar image
        (healthBarImage.transform as RectTransform).anchoredPosition = localPoint;
    }

    // Update the health bar position every frame to follow the enemy
    private void Update()
    {
        UpdateHealthBarPosition();
    }

    // Enemy basic attack method (called when the enemy attacks the player)
    public void BasicAttack(PlayerController player, CombatManager combatManager)
    {

        animator.SetTrigger("Attack"); // Play the "Attack" animation
        int toHitRoll = Random.Range(1, 21); // Roll a d20 to determine if the attack hits
        if (toHitRoll >= player.armorClass)
        {
            int damage = Random.Range(1, 11); // Deal damage between 1 and 10
            player.TakeDamage(damage); // Call the player's TakeDamage method
            combatManager.UpdateFeedback($"Enemy attacks for {damage} damage!");
        }
        else
        {
            combatManager.UpdateFeedback("Enemy's attack missed!");
        }
    }

    // Roll initiative for the enemy, returning a value between 1 and 20
    public int RollInitiative() => Random.Range(1, 21);
}
