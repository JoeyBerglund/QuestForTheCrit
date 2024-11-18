using UnityEngine;
using UnityEngine.UI;

public class EnemyController : Character
{
    public int health = 100, maxHealth = 100, attackPower = 10, armorClass = 12;
    private Image healthBarImage;
    private Canvas healthBarCanvas; // Store reference to the canvas

    public override bool isAlive => health > 0;

    // This method is called when the enemy takes damage
    public override void TakeDamage(int damage)
    {
        health = Mathf.Max(health - damage, 0);
        UpdateHealthBar(); // Update the health bar after taking damage
    }

    // Set the health bar image and update the health bar
    public void SetHealthBar(Image healthBarFill)
    {
        healthBarImage = healthBarFill; // Set the reference to HealthBarfill image
        UpdateHealthBar(); // Ensure health bar is updated immediately
    }

    // Initialize the health bar position by setting the canvas reference
    public void InitializeHealthBarPosition(Canvas canvas)
    {
        healthBarCanvas = canvas; // Set the reference to the health bar canvas from the CombatManager
    }

    // Update the health bar's fill amount
    public void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.transform.Find("HealthBarFill").GetComponent<Image>().fillAmount = Mathf.Clamp01((float)health / maxHealth); // Set the health bar fill based on current health
        }
    }

    // Update the health bar position to follow the enemy in world space
    void UpdateHealthBarPosition()
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

    // Update health bar position in each frame to follow the enemy
    void Update()
    {
        UpdateHealthBarPosition(); // Update position of health bar every frame
    }

    // Basic attack function (called when enemy attacks the player)
    public void BasicAttack(PlayerController player, CombatManager combatManager)
    {
        int toHitRoll = Random.Range(1, 21);  // Roll a d20 to determine if the attack hits
        if (toHitRoll >= player.armorClass)
        {
            int damage = Random.Range(1, 11);  // Deal damage between 1 and 10
            player.TakeDamage(damage);  // Call the player's TakeDamage method
            combatManager.UpdateFeedback("Enemy attacks for " + damage + " damage!");
        }
        else
        {
            combatManager.UpdateFeedback("Enemy's attack missed!");
        }
    }

    // Roll initiative for the enemy, a value between 1 and 20
    public int RollInitiative() => Random.Range(1, 21);
}
