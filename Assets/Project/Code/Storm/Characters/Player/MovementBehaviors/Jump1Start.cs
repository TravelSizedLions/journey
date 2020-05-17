using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace Storm.Characters.Player {
  public class Jump1Start : HorizontalMotion {

    private void Awake() {
      AnimParam = "jump_1_start";
    }


    #region Movement Behavior Implementation
    //-------------------------------------------------------------------------
    // Movement Behavior Implementation
    //-------------------------------------------------------------------------

    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump")) {
        ChangeToState<Jump2Start>();
      }
    }




    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

    }

    public void OnSingleJumpFinished() {
      ChangeToState<Jump1Rise>();
    }


    public override void OnStateExit() {
      MovementSettings settings = GetComponent<MovementSettings>();

      Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
      playerRB.velocity = (playerRB.velocity * Vector2.right) + new Vector2(0, settings.SingleJumpForce);
    }
    #endregion
  }

}