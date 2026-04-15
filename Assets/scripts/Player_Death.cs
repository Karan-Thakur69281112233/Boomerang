using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Death : MonoBehaviour {
    [Header("Scene Settings")]
    public string loseSceneName = "loose";

    [Header("Detection Tags")]
    public string terrainTag = "Terrain";
    //public string waterTag = "Water";

    // 1. Handles hitting the solid Terrain (Character Controller specific)
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.CompareTag(terrainTag)) {
            HandleDeath();
        }
    }

    // 2. Handles falling into the Water (Trigger)
    //private void OnTriggerEnter(Collider other) {
    //    if (other.CompareTag(waterTag)) {
    //        HandleDeath();
    //    }
    //}

    // The shared logic for dying
    private void HandleDeath() {
        Debug.Log("Player Died! Freezing time and loading scene... player collided with terrain");

        // Stop all physics/movement/time
        Time.timeScale = 0f;

        // Load the lose scene
        SceneManager.LoadScene(loseSceneName);
    }
}