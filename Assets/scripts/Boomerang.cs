using UnityEngine;

public class Boomerang : MonoBehaviour {
    [Header("Flight Settings")]
    public float flightSpeed = 1.5f;
    public float rotationSpeed = 800f;
    public float curveWidth = 10f;
    public float maxCurveTime = 1.0f; // The "X" amount of time before auto-teleport
    public LayerMask collisionMask = ~0;

    private bool isThrown = false;
    private Vector3 startPoint, controlPoint, endPoint;
    private float curveTime = 0f;

    private Transform handTransform;
    private Transform playerTransform;
    private CharacterController playerController;
    private float playerHalfHeight;

    public bool IsThrown => isThrown;

    public void Initialize(Transform hand, CharacterController cc) {
        handTransform = hand;
        playerController = cc;
        playerTransform = cc.transform;
        playerHalfHeight = cc.height / 2f + 0.1f;

        if (playerController != null) {
            Physics.IgnoreCollision(GetComponent<Collider>(), playerController.GetComponent<Collider>(), true);
        }
        Catch();
    }
    
    public void Throw(Vector3 direction, float distance) {
        isThrown = true;
        curveTime = 0f;
        transform.parent = null;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = true;

        startPoint = transform.position;
        endPoint = startPoint + (direction * distance);

        Vector3 midPoint = (startPoint + endPoint) / 2;
        Vector3 stableRight = Vector3.Cross(Vector3.up, direction).normalized;
        controlPoint = midPoint + (stableRight * curveWidth);
    }

    private void InstantTeleport(Vector3 hitLocation) {
        if (playerController != null) {
            Vector3 safeLandingPos = hitLocation + Vector3.up * playerHalfHeight;
            playerController.enabled = false;
            playerTransform.position = safeLandingPos;
            playerController.enabled = true;
        }
        Catch();
    }

    public void Catch() {
        isThrown = false;
        transform.parent = handTransform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }

    void Update() {
        if (!isThrown) return;

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        curveTime += Time.deltaTime * flightSpeed;
        Vector3 nextPos = CalculateBezierPoint(curveTime, startPoint, controlPoint, endPoint);

        // Check for collision during flight
        Vector3 dir = (nextPos - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, nextPos);

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dist, collisionMask)) {
            if (!hit.collider.CompareTag("Player")) {
                InstantTeleport(hit.point);
                return;
            }
        }

        transform.position = nextPos;

        // MANDATORY TELEPORT: If time is up, teleport to current air position
        if (curveTime >= maxCurveTime) {
            InstantTeleport(transform.position);
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }
}