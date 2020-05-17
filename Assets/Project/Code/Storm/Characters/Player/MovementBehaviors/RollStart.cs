using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class RollStart : HorizontalMotion {

    private void Awake() {
      AnimParam = "roll_start";
    }

    public override void OnUpdate() {
      if (Input.GetButton("Jump")) {
        ChangeToState<Jump1Start>();
      }
    }

    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (Mathf.Abs(playerRB.velocity.x) < idleThreshold) {
        ChangeToState<CrouchEnd>();
      }     
    }

    public void OnRollStartFinished() {
      if (Input.GetAxis("Horizontal") == 0) {
        if (Input.GetButton("Down")) {
          ChangeToState<Crouching>();
        } else {
          ChangeToState<CrouchEnd>();
        }
      } else {
        if (Input.GetButton("Down")) {
          ChangeToState<Crawling>();
        } else {
          ChangeToState<RollEnd>();
        }
      }
    }


    public override void OnStateEnter() {
      Debug.Log("Enter roll!");
    }

  }

}