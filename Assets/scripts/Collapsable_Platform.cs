using UnityEngine;
using System.Collections;

public class Collapsable_Platform : MonoBehaviour {
    [Header("Settings")]
    public float timeToBreak = 3f;
    // Removed activationTag field

    [Header("Visual Feedback (Optional)")]
    public Material warningMaterial;

    private bool timerStarted = false;
    private Renderer platformRenderer;
    private Material originalMaterial;

    void Awake() {
        platformRenderer = GetComponent<Renderer>();
        if (platformRenderer != null) {
            originalMaterial = platformRenderer.material;
        }
    }

    // Called when the platform is touched by another non-trigger collider
    private void OnCollisionEnter(Collision collision) {
        // 1. Check if the timer hasn't started yet (Tag check removed!)
        if (!timerStarted) {
            timerStarted = true;

            Debug.Log(gameObject.name + " contacted by " + collision.gameObject.name + ". Starting destruction timer for " + timeToBreak + " seconds.");

            StartCoroutine(BreakAfterDelay());
        }
    }

    public void Activate() {
        // Ensure the timer only starts once
        if (!timerStarted) {
            timerStarted = true;

            // Use Debug.Log for confirmation
            Debug.Log(gameObject.name + " activated by player via ControllerHit. Starting destruction timer for " + timeToBreak + " seconds.");

            // Start the Coroutine
            StartCoroutine(BreakAfterDelay());
        }
    }

    IEnumerator BreakAfterDelay() {
        if (warningMaterial != null && platformRenderer != null) {
            platformRenderer.material = warningMaterial;
        }

        yield return new WaitForSeconds(timeToBreak);

        Debug.Log("Destroying platform " + gameObject.name + " after " + timeToBreak + " seconds.");

        Destroy(gameObject);
    }
}