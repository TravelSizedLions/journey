using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class Running : MovementBehavior {

    #region Fields
    private float idleThreshold = 0.01f;

    #region References 

    private new Rigidbody2D rigidbody;

    private HorizontalMotion motion;

    #endregion

    #endregion


    #region Unity API
    private void Awake() {
      AnimParam = "running";

      motion = GetComponent<HorizontalMotion>();
    }
    #endregion


    #region Movement Behavior Implementation
    //-------------------------------------------------------------------------
    // Movement Behavior Implementation
    //-------------------------------------------------------------------------

    public override void OnStateEnter(PlayerCharacter player) {
      base.OnStateEnter(player);
      rigidbody = player.rigidbody;
    }

    public override void HandleInput() {
      if (Input.GetButtonDown("Jump")) {
        ChangeState<SingleJump>();
      }
    }

    public override void HandlePhysics() {

      Facing facing = motion.Handle();

      if (Mathf.Abs(rigidbody.velocity.x) < idleThreshold) {
        if (Input.GetButton("Down")) {
          ChangeState<StartCrouch>();
        } else {
          ChangeState<Idle>();
        }
      } else {
        if (Input.GetButton("Down")) {
          ChangeState<Dive>();
        } else {
          player.SetFacing(facing);
        }
      }
    }


    private void OnCollisionExit2D(Collision2D collision) {
      if (!player.IsTouchingGround()) {
        ChangeState<Fall>();
      }
    }
    #endregion
  }
}