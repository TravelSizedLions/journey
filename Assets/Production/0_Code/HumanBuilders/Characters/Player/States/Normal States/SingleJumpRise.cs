using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// When the player is rising during their first jump.
  /// </summary>
  public class SingleJumpRise : HorizontalMotion {
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
    private string param = "jump_1_rise";
    #endregion
 

    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (!base.TryBufferedJump()) {
          ChangeToState<DoubleJumpStart>();
        }
      } else if (player.PressedAction() || player.PressedAltAction()) {
        player.Interact();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();

      player.SetFacing(facing);
      
      if (player.IsFalling()) {
        ChangeToState<SingleJumpFall>();
      } else {
        bool leftWall = player.IsTouchingLeftWall();
        bool rightWall = player.IsTouchingRightWall();
        if ((leftWall || rightWall)) {
          ChangeToState<WallSlide>();
        }
      }
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        Carriable item = obj.GetComponent<Carriable>();
        item.OnPickup();
        ChangeToState<CarryJumpRise>();
      } else if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }
    #endregion
  }
}
