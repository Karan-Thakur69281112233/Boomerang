using UnityEngine;

public class Trampoline : MonoBehaviour {
    public float bounceVelocity = 15f;

    private void OnTriggerEnter(Collider other) {
        // 1. Get the movement script for the bounce
        player_movement movement = other.GetComponent<player_movement>();
        // 2. Get the attack script for the throw reset
        PlayerAttack attack = other.GetComponent<PlayerAttack>();

        if (movement != null) {
            movement.ApplyVerticalForce(bounceVelocity);
        }

        if (attack != null) {
            attack.ResetAirThrows();
        }

        Debug.Log("Trampoline: Bounced and Refilled Throws!");
    }
}