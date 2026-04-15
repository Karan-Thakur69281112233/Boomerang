using UnityEngine;

public class player_animation : MonoBehaviour {
    Animator anim;
    public player_movement movement; // Assign in inspector or find automatically

    void Start() {
        if (movement == null)
            movement = GetComponentInParent<player_movement>();
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (movement != null && anim != null) {
            anim.SetInteger("state", movement.State);
        }
    }
}
