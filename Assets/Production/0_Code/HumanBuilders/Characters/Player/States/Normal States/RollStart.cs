using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// When the player enters into a dive roll.
  /// </summary>
  public class RollStart : HorizontalMotion {
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
    private string param = "roll_start";
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

      if (Mathf.Abs(physics.Vx) < settings.IdleThreshold) {
        ChangeToState<CrouchEnd>();
      }     
    }


    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnRollStartFinished() {
      // If the state hasn't already transitioned to some other state.
      if (!exited) {
        
        // If the player isn't trying to move.
        if (player.GetHorizontalInput() == 0) {
          if (player.HoldingDown()) {
            ChangeToState<Crouching>();
          } else {
            ChangeToState<CrouchEnd>();
          }

        // If the player is trying to move.
        } else {
          if (player.CanMove() && player.HoldingDown()) {
            ChangeToState<Crawling>();
          } else {
            ChangeToState<RollEnd>();
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