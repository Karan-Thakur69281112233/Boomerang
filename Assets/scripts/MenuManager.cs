using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    [Header("Scene Settings")]
    public string gameSceneName = "game";

    // This runs automatically as soon as the scene opens
    void Start() {
        // 1. UNLOCK the mouse cursor so it can move
        Cursor.lockState = CursorLockMode.None;

        // 2. MAKE IT VISIBLE so you can see it to click your buttons
        Cursor.visible = true;

        // 3. Ensure the game is not frozen
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Loads the main game scene when the Start/Restart button is clicked.
    /// </summary>
    public void StartGame() {
        // Reset time scale to 1 to ensure the game isn't paused
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame() {
        Debug.Log("Quitting application...");
        Application.Quit();
    }
}