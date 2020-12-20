using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is exiting a dive roll.
  /// </summary>
  public class RollEnd : HorizontalMotion {
    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "roll_end";
    #endregion

    #region Player State API

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
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
        ChangeToState<Land>();
      }
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnRollEndFinished() {
      // If the state hasn't already transition to some other state.
      if (!exited) {

        // If the player isn't moving left/right.
        if (!player.TryingToMove()) {
          if (player.HoldingDown()) {
            ChangeToState<Crouching>();
          } else {
            ChangeToState<Idle>();
          }

        // If the player's trying to move, and isn't prevented from moving.
        } else if (player.CanMove()) {
          
          if (player.HoldingDown()) {
            ChangeToState<Crawling>();
          } else {
            ChangeToState<Running>();
          }
        }

      }
    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }
    #endregion

  }

}