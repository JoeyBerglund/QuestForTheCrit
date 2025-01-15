using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public float moveSpeed = 5f;          // Player's movement speed
    public float jumpHeight = 5f;         // How high the player can jump
    public Transform groundCheck;         // Position to check if player is on the ground
    public LayerMask groundLayer;         // Ground layer mask to detect if the player is grounded
    public float skinWidth = 0.1f;        // Extra margin to prevent clipping
    public float airControlFactor = 0.5f; // Factor to reduce air control, you can adjust this value for more/less control in the air

    private float horizontalInput;        // Store horizontal movement input (A/D or Left/Right)
    private bool isGrounded;              // Check if the player is on the ground
    private Vector3 customVelocity;       // Renamed to customVelocity to avoid ambiguity
    private float horizontalVelocity;     // Store the player's horizontal velocity
    private BoxCollider playerCollider;   // Reference to the player's collider
    private Vector3 moveDirection;        // Store the direction of movement

    private void Start()
    {
        playerCollider = GetComponent<BoxCollider>(); // Assuming you're using a BoxCollider for the player
    }

    private void Update()
    {
        // Handle player movement
        horizontalInput = Input.GetAxis("Horizontal"); // Left/Right arrow keys or A/D keys

        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);

        // Handle movement on the X-axis (2.5D movement)
        MovePlayer();

        // Handle jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Apply gravity (constant downward force)
        ApplyGravity();
    }

    private void MovePlayer()
    {
        // Calculate the intended move direction (X-axis movement only)
        moveDirection = new Vector3(horizontalInput, 0f, 0f); // Only move on X-axis for 2.5D

        // If grounded, we apply normal movement speed
        if (isGrounded)
        {
            horizontalVelocity = moveDirection.x * moveSpeed;
        }
        else
        {
            // If in the air, we allow a fraction of the horizontal speed (air control factor)
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, moveDirection.x * moveSpeed, airControlFactor);
        }

        // Apply the horizontal velocity to the player's movement
        Vector3 targetPosition = transform.position + new Vector3(horizontalVelocity, 0f, 0f) * Time.deltaTime;

        // Cast a ray to check for potential collisions on the X-axis (left/right)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, moveDirection, out hit, playerCollider.bounds.extents.x + skinWidth))
        {
            // If a wall is detected, do not allow movement
            return;
        }

        // If no wall, move player normally
        transform.Translate(new Vector3(horizontalVelocity, 0f, 0f) * Time.deltaTime);
    }

    private void Jump()
    {
        // Set upward velocity for jumping
        customVelocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y); // Using the physics gravity value
    }

    private void ApplyGravity()
    {
        // Apply gravity manually (affects only Y-axis)
        if (!isGrounded)
        {
            customVelocity.y += Physics.gravity.y * Time.deltaTime; // Apply gravity when not grounded
        }
        else if (customVelocity.y < 0)
        {
            customVelocity.y = 0f; // Prevent downward velocity when on the ground
        }

        // Apply vertical velocity to the player
        transform.Translate(customVelocity * Time.deltaTime);
    }
}
