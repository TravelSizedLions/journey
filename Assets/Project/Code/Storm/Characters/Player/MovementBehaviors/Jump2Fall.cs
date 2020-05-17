using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class Jump2Fall : HorizontalMotion {
    
    private float fallTimer;

    private float rollOnLand;

    private void Awake() {
      AnimParam = "jump_2_fall";
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
          ChangeToState<RollStart>();
        } else {
          ChangeToState<Land>();
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