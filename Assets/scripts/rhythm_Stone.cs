using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Rhythm_Stone : MonoBehaviour {
    [Header("Timing Settings")]
    public float activeTime = 1.5f;    // Time it is solid
    public float hiddenTime = 0.5f;    // Time it is gone (the gap)
    public float warningDuration = 1f; // How long it stays red before vanishing

    [Header("Visual Warning")]
    public Color warningColor = Color.red;

    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    private Rigidbody rb;
    private Color originalColor;

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        // Set physics to be solid and unmoving
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        if (meshRenderer != null) {
            originalColor = meshRenderer.material.color;
        }
    }

    void Start() {
        // Start the infinite loop
        StartCoroutine(RhythmLoop());
    }

    IEnumerator RhythmLoop() {
        while (true) {
            // 1. ACTIVE STATE
            // Stay in the original color for most of the active time
            if (meshRenderer != null) meshRenderer.material.color = originalColor;
            yield return new WaitForSeconds(activeTime - warningDuration);

            // 2. WARNING STATE
            // Turn red to warn the player it's about to vanish
            if (meshRenderer != null) meshRenderer.material.color = warningColor;
            yield return new WaitForSeconds(warningDuration);

            // 3. HIDDEN STATE (The 0.2s Gap)
            // Disable visuals and collision
            if (meshRenderer != null) meshRenderer.enabled = false;
            boxCollider.enabled = false;

            yield return new WaitForSeconds(hiddenTime);

            // 4. RESTORE STATE
            // Bring it back and repeat the loop
            if (meshRenderer != null) meshRenderer.enabled = true;
            boxCollider.enabled = true;
        }
    }
}