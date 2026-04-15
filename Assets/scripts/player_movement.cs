

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class player_movement : MonoBehaviour {

    // Public state for the animator
    public int State { get { return state; } }
    private int state = 0; // 0=idle, 1=walk, 2=walk-back, 3=fall/jump

    // Player/Camera Facing
    private float facingYaw = 0f;

    [Header("Speeds")]
    public float walkSpeed = 4f;

    [Header("Jump & Gravity")] // Header updated
    public float gravity = -25f;
    public float jumpHeight = 1.6f; // JUMP HEIGHT RESTORED

    [Header("Ground Check")]
    public float groundCheckOffset = 0.1f;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer = ~0;

    // Runtime
    private CharacterController controller;
    private bool isGrounded;
    private Vector3 verticalVelocity;
    private Vector3 moveDir;

    void Awake() {
        controller = GetComponent<CharacterController>();
    }

    // Public method for a camera controller to call
    public void SetFacingYaw(float yawAngle) {
        facingYaw = yawAngle;
        transform.rotation = Quaternion.Euler(0f, facingYaw, 0f);
    }

    void Update() {
        // --- 1. CHECKS ---
        GroundCheck();

        // --- 2. INPUT & MOVEMENT CALCULATION ---
        HandleMovement();
        HandleJump(); // JUMP CALL RESTORED

        // --- 3. APPLY PHYSICS & MOVEMENT ---
        ApplyGravity();

        // Apply movement (Normal movement + Gravity)
        Vector3 totalMove = moveDir * Time.deltaTime + verticalVelocity * Time.deltaTime;
        controller.Move(totalMove);

        // --- 4. STATE LOGIC ---
        UpdateAnimationState();
    }

    // NEW FUNCTION: JUMP LOGIC RESTORED
    // SUGGESTED IMPROVEMENT for HandleJump()

    void HandleJump() {
        if (isGrounded && Input.GetButtonDown("Jump")) {
            // Standard formula to achieve desired jump height based on gravity
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            // ADD THESE TWO LINES for consistency with ApplyVerticalForce:
            isGrounded = false;
            state = 3;
        }
    }


    // In your player_movement.cs file, add this method:

    // player_movement.cs

    /// <summary>
    /// Public method called by external objects (like the Trampoline).
    /// </summary>
    /// <param name="verticalVelocity">The upward speed to apply.</param>
    public void ApplyVerticalForce(float verticalVelocity) {
        // 1. Overwrite the current vertical velocity with the bounce force.
        this.verticalVelocity.y = verticalVelocity;

        // 2. CRITICAL FIX: Manually set isGrounded to false 
        // to force the system into the airborne state immediately.
        isGrounded = false;

        // 3. Set the animation state to Jump/Fall (3)
        state = 3;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // Try to get the Collapsable_Platform component from the object we hit
        Collapsable_Platform platform = hit.gameObject.GetComponent<Collapsable_Platform>();

        // If the platform exists and we hit it from above (i.e., we are standing on it)
        if (platform != null) {
            // Check if the collision was mostly downward (prevents walls from triggering it)
            if (hit.normal.y > 0.8f) // 0.8f is a good threshold for "mostly top surface"
            {
                platform.Activate();
            }
        }
    }

    void GroundCheck() {
        // 1. Define the sphere check position
        Vector3 spherePosition = new Vector3(
            transform.position.x,
            transform.position.y - (controller.height / 2) + controller.radius - groundCheckOffset,
            transform.position.z
        );

        // 2. Perform the check
        isGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);

        // 3. Ground stickiness logic (The FIX)
        if (isGrounded) {
            // If we hit the ground, we MUST cancel any massive negative velocity.
            // We set it to a tiny negative value to force the CharacterController
            // to stay "stuck" to the ground without clipping through.
            if (verticalVelocity.y < 0f) {
                verticalVelocity.y = -0.5f; // Set a small, safe downward velocity
            }
        }
    }

    void HandleMovement() {
        // Use GetAxisRaw for snappy, simple input
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
        float vertical = Input.GetAxisRaw("Vertical");    // W/S

        // Calculate direction relative to the player's facing direction (transform.forward)
        Vector3 direction = (transform.forward * vertical + transform.right * horizontal).normalized;

        // Set the final moveDir using only walkSpeed
        if (direction.magnitude >= 0.1f) {
            moveDir = direction * walkSpeed;
        } else {
            moveDir = Vector3.zero; // No input, no movement
        }
    }

    void ApplyGravity() {
        // Apply gravity constantly
        if (!isGrounded) {
            verticalVelocity.y += gravity * Time.deltaTime;
        }
    }

    void UpdateAnimationState() {
        bool isMovingBackward = false;
        if (moveDir.magnitude > 0.01f) {
            isMovingBackward = Vector3.Dot(moveDir.normalized, transform.forward) < -0.5f;
        }

        // State logic: priority-based
        if (!isGrounded) {
            state = 3; // Fall/Jump state
        } else { // Is Grounded
            if (moveDir.magnitude < 0.1f) {
                state = 0; // Idle
            } else if (isMovingBackward) {
                state = 2; // Walk Back
            } else {
                state = 1; // Walk Forward
            }
        }
    }

    void OnDrawGizmosSelected() {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (controller == null) return;

        Gizmos.color = Color.yellow;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - (controller.height / 2) + controller.radius - groundCheckOffset, transform.position.z);
        Gizmos.DrawWireSphere(spherePosition, groundCheckRadius);
    }
}