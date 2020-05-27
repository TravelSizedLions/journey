using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is exiting a dive roll.
  /// </summary>
  public class RollEnd : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "roll_end";
    }
    #endregion

    #region Player State API

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (Input.GetButton("Jump") && player.CanJump()) {
        ChangeToState<Jump1Start>();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (Mathf.Abs(rigidbody.velocity.x) < idleThreshold) {
        ChangeToState<Land>();
      }
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      Debug.Log("Exit roll!");
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnRollEndFinished() {
      float xVel = rigidbody.velocity.x;
      if (!player.CanMove() || Input.GetAxis("Horizontal") == 0) {
        if (Input.GetButton("Down")) {
          ChangeToState<Crouching>();
        } else {
          ChangeToState<Idle>();
        }
      } else if (player.CanMove()) {
        if (Input.GetButton("Down")) {
          ChangeToState<Crawling>();
        } else {
          ChangeToState<Running>();
        }
      }
    }
    #endregion

  }

}