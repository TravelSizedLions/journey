using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class StartCrouch : MovementBehavior {

    private float idleThreshold;

    private void Awake() {
      AnimParam = "crouch_in";
    }

    private void Start() {
      MovementSettings settings = GetComponent<MovementSettings>();
      idleThreshold = settings.IdleThreshold;
    }

    public void OnAnimationFinished() {
      ChangeState<Crouching>();
    }

    public override void HandleInput() {
      if (!Input.GetButton("Down")) {
        ChangeState<EndCrouch>();
      }
    }

    public override void HandlePhysics() {
      float input = Input.GetAxis("Horizontal");
      if (input != 0) {
        ChangeState<Crawling>();
      }
    }

  }

}