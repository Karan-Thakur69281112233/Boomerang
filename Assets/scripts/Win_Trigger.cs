using UnityEngine;
using UnityEngine.SceneManagement;

public class Win_Trigger : MonoBehaviour {
    [Header("Scene Settings")]
    public string winSceneName = "win"; // Ensure this matches your Scene List exactly

    private void OnTriggerEnter(Collider other) {
        // Check if the object is the player
        if (other.CompareTag("Player")) {
            Debug.Log("Player reached the goal! Moving to Win scene.");

            // Unlock cursor here too just in case
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Load the 'win' scene
            SceneManager.LoadScene(winSceneName);
        }
    }
}