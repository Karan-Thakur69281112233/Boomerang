using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    [Tooltip("Drag empty GameObjects here to define the path.")]
    public Transform[] waypoints;
    public float speed = 2.0f;
    [Tooltip("Time the platform pauses before moving to the next point.")]
    public float waitTime = 1.0f;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    // CRITICAL: We need a variable to store the displacement to use in OnTriggerStay
    private Vector3 displacement;
    private Vector3 previousPosition;

    void Update() {
        if (waypoints.Length == 0) return;

        if (isWaiting) {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f) isWaiting = false;
        }
    }

    void FixedUpdate() {
        if (waypoints.Length == 0 || isWaiting) return;

        // 1. Store the position BEFORE movement
        previousPosition = transform.position;

        MoveToNextWaypoint();

        // 2. Calculate and store the displacement
        // This vector represents the platform's movement for this FixedUpdate tick.
        displacement = transform.position - previousPosition;
    }

    void MoveToNextWaypoint() {
        Vector3 target = waypoints[currentWaypointIndex].position;

        // IMPORTANT: Use Time.fixedDeltaTime here for movement in FixedUpdate
        // Using Time.deltaTime in FixedUpdate can lead to unstable movement.
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, target) < 0.001f) {
            isWaiting = true;
            waitTimer = waitTime;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // --- PLAYER CARRYING LOGIC ---

    // 1. ENTER: No change needed.
    private void OnTriggerEnter(Collider other) {
        // No parenting here.
    }

    // 2. STAY: Apply the platform's pre-calculated displacement.
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            CharacterController cc = other.GetComponent<CharacterController>();

            // The logic is now cleaner and more focused:
            // Only move the player if the CharacterController component exists.
            // We can often skip the cc.isGrounded check for stability, as 
            // cc.Move() is designed to handle it, but keeping it makes it safer.
            if (cc != null && cc.isGrounded) {
                // Apply the platform's calculated movement (force/speed)
                cc.Move(displacement);
            }
        }
    }

    // 3. EXIT: No change needed.
    private void OnTriggerExit(Collider other) {
        // No unparenting.
    }
}