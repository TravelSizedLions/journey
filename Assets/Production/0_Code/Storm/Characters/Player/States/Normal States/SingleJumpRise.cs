using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is rising during their first jump.
  /// </summary>
  public class SingleJumpRise : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "jump_1_rise";
    }
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
        bool rightWall =player.IsTouchingRightWall();
        if ((leftWall || rightWall) && !IsWallJumping()) {
          ChangeToState<WallRun>();
        }
      }
    }

    public override void OnSignal(GameObject obj) {
      Carriable carriable = obj.GetComponent<Carriable>();
      if (carriable != null) {
        carriable.OnPickup();
        ChangeToState<CarryJumpRise>();
      }
    }
    #endregion
  }
}
