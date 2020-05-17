using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class RollEnd : HorizontalMotion {

    private void Awake() {
      AnimParam = "roll_end";
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
        ChangeToState<Land>();
      }
    }

    public void OnRollEndFinished() {
      float xVel = playerRB.velocity.x;
      if (Input.GetAxis("Horizontal") == 0) {
        if (Input.GetButton("Down")) {
          ChangeToState<Crouching>();
        } else {
          ChangeToState<Idle>();
        }
      } else {
        if (Input.GetButton("Down")) {
          ChangeToState<Crawling>();
        } else {
          ChangeToState<Running>();
        }
      }
    }

    public override void OnStateEnter() {
      Debug.Log("Exit roll!");
    }


  }

}