using UnityEngine;

public class camera_movement : MonoBehaviour {
    public Transform player;           // Assign your player Transform here
    public Vector3 offset = new Vector3(0, 2, -7);  // Camera offset from player
    public float mouseSensitivity = 5f;
    public float pitchMin = -30f;
    public float pitchMax = 65f;

    float yaw = 0f;
    float pitch = 10f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate() {
        // Mouse look
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // Camera rotation and position
        Vector3 targetPos = player.position;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredCameraPos = targetPos + rotation * offset;
        transform.position = desiredCameraPos;
        transform.LookAt(targetPos + Vector3.up * 1.1f); // focus slightly above pivot

        // Update player facing direction
        // The player script needs a public method to handle direction update
        player.GetComponent<player_movement>().SetFacingYaw(yaw);
    }
}
