using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerAttack : MonoBehaviour {
    [Header("References")]
    public Transform cameraTransform;
    public Boomerang boomerangScript;
    public Transform handPosition;
    public CharacterController charController;

    // Reference to your movement script
    private player_movement movementScript;

    [Header("Throw Settings")]
    public float fixedThrowDistance = 20f;
    public int trajectoryPoints = 20;
    public int maxAirThrows = 1;

    private int throwsRemaining;
    private LineRenderer lr;

    void Start() {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        throwsRemaining = maxAirThrows;

        // Find the player_movement script on this object
        movementScript = GetComponent<player_movement>();

        if (boomerangScript != null && handPosition != null) {
            boomerangScript.Initialize(handPosition, charController);
        }
    }

    void Update() {
        // 1. RESET CHARGES: 
        // We use the State property from your script. 
        // If state is NOT 3 (Jump/Fall), the player is on a stone/ground.
        if (movementScript != null && movementScript.State != 3) {
            throwsRemaining = maxAirThrows;
        }

        // 2. THROW LOGIC
        bool canThrow = !boomerangScript.IsThrown && throwsRemaining > 0;

        if (canThrow) {
            // Show trajectory while holding button
            if (Input.GetMouseButton(0)) {
                DrawTrajectory();
            }

            // Fire on release
            if (Input.GetMouseButtonUp(0)) {
                boomerangScript.Throw(cameraTransform.forward, fixedThrowDistance);
                throwsRemaining--;
                lr.enabled = false;
            }
        } else {
            lr.enabled = false;
        }
    }
    // Add this inside your PlayerAttack class
    public void ResetAirThrows() {
        throwsRemaining = maxAirThrows;
        Debug.Log("Air Throws Reset by Trampoline!");
    }
    void DrawTrajectory() {
        lr.enabled = true;
        lr.positionCount = trajectoryPoints;

        Vector3 start = handPosition.position;
        Vector3 end = start + (cameraTransform.forward * fixedThrowDistance);

        Vector3 mid = (start + end) / 2;
        Vector3 stableRight = Vector3.Cross(Vector3.up, cameraTransform.forward).normalized;
        Vector3 control = mid + (stableRight * boomerangScript.curveWidth);

        for (int i = 0; i < trajectoryPoints; i++) {
            float t = i / (float)(trajectoryPoints - 1);
            Vector3 point = CalculateBezierPoint(t, start, control, end);
            lr.SetPosition(i, point);
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }
}