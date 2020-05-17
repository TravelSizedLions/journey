using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class WallSlide : HorizontalMotion {

    private float wallSlideDeceleration;

    private void Awake() {
      AnimParam = "wall_slide";
    }

    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump")) {
        ChangeToState<WallJump>();
      }
    }

    public override void OnFixedUpdate() {  
      Facing facing = MoveHorizontally();
      
      bool leftWall = player.IsTouchingLeftWall();
      bool rightWall = player.IsTouchingRightWall();

      if (!(leftWall || rightWall)) {
        ChangeToState<Jump1Fall>();
        return;
      } else if (player.IsTouchingGround()) {
        ChangeToState<Idle>();
        return;
      } else {
        float input = Input.GetAxis("Horizontal");
        if ((leftWall && input < 0) || (rightWall && input > 0)) {
          playerRB.velocity =  new Vector2(0, playerRB.velocity.y*wallSlideDeceleration); 
        } else {
          playerRB.velocity =  new Vector2(playerRB.velocity.x, playerRB.velocity.y*wallSlideDeceleration); 
        }
      }

      player.SetFacing(facing);
    }

    public override void OnStateAdded() {
      base.OnStateAdded();

      playerRB = GetComponent<Rigidbody2D>();

      MovementSettings settings = GetComponent<MovementSettings>();
      wallSlideDeceleration = 1-settings.WallSlideDeceleration;
    }

  }
}