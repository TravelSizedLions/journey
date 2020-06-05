using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// When the player enters into a dive roll.
  /// </summary>
  public class RollStart : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "roll_start";
    }
    #endregion

    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.HoldingJump()) {
        ChangeToState<SingleJumpStart>();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (Mathf.Abs(physics.Vx) < idleThreshold) {
        ChangeToState<CrouchEnd>();
      }     
    }

    // /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    // /// </summary>
    // public override void OnStateEnter() {
    //   Debug.Log("Enter roll!");
    // }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnRollStartFinished() {
      if (player.GetHorizontalInput() == 0) {
        if (player.HoldingDown()) {
          ChangeToState<Crouching>();
        } else {
          ChangeToState<CrouchEnd>();
        }
      } else {
        if (player.CanMove() && player.HoldingDown()) {
          ChangeToState<Crawling>();
        } else {
          ChangeToState<RollEnd>();
        }
      }
    }
    #endregion
  }
}