using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is standing still.
  /// </summary>
  public class Idle : PlayerState {

    #region Unity API
    private void Awake() {
      AnimParam = "idle";
    }
    #endregion

    #region Player State API

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.TryingToJump()) {
        if (player.IsTouchingRightWall() || player.IsTouchingLeftWall()) {
          ChangeToState<WallRun>();
        } else {
          ChangeToState<Jump1Start>();
        }
      } else if (Input.GetButton("Down")) {
        ChangeToState<CrouchStart>();
      } else if (player.TryingToMove()) {
        ChangeToState<Running>();
      }
    }


    public override void OnStateEnter() {
      rigidbody.velocity = Vector2.zero;
    }

    #endregion
  }

}