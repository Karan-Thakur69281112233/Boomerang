using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class Loose_Trigger : MonoBehaviour {
    [Header("Scene Settings")]
    public string loseSceneName = "loose"; // Make sure this matches your scene name exactly

    private void OnTriggerEnter(Collider other) {
        // Check if the object is the player
        if (other.CompareTag("Player")) {
            Debug.Log("Player hit the water! Moving to Loose scene.");
            Time.timeScale = 0f;
            // Load the 'Loose' scene
            SceneManager.LoadScene(loseSceneName);
        }
    }
}


