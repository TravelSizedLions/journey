using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Jump1Fall : HorizontalMotion {


    private float rollOnLand;

    private float fallTimer;

    private void Awake() {
      AnimParam = "jump_1_fall";
    }

    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump")) {
        ChangeToState<Jump2Start>();
      }
    }

    public override void OnFixedUpdate() {
      fallTimer += Time.deltaTime;

      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        ChangeToState<WallSlide>();
      } else if (player.IsTouchingGround()) {
        float xVel = playerRB.velocity.x;

        if (Mathf.Abs(xVel) > idleThreshold) {
          if (fallTimer > rollOnLand) {
            ChangeToState<RollStart>();
          } else {
            if (Input.GetButton("Down")) {
              ChangeToState<CrouchStart>();
            } else {
              ChangeToState<Land>();
            }
          }
        } else {
          if (fallTimer > rollOnLand) {
            ChangeToState<CrouchEnd>();
          } else  {
            ChangeToState<Land>();
          }
        }
      }
    }

    public override void OnStateAdded() {
      base.OnStateAdded();

      MovementSettings settings = GetComponent<MovementSettings>();
      rollOnLand = settings.RollOnLand;
    }


    public override void OnStateEnter() {
      fallTimer = 0;
    }

  }
}