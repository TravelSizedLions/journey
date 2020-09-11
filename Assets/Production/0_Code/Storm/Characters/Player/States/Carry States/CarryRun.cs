using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is running while carrying an item.
  /// </summary>
  public class CarryRun : CarryMotion {

    #region Fields 
    /// <summary>
    /// Whether or not the player has released the action button since entering
    /// this state.
    /// </summary>
    private bool releasedAction;
    
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "carry_run";
    }
    #endregion

    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();
      } else if (player.PressedDown()) {
        ChangeToState<CarryCrouchStart>();
      }

      if (player.ReleasedAction()) {
        releasedAction = true;
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (facing == Facing.None) {
        ChangeToState<CarryIdle>();
      } else if (!player.IsTouchingGround() && player.IsFalling()) {
        player.StartCoyoteTime();
        ChangeToState<CarryJumpFall>();
      } else if (releasedAction && player.HoldingAction()) {
        ChangeToState<ThrowItem>();
      }
    }


    public override void OnStateEnter() {
      releasedAction = player.ReleasedAction() || !player.HoldingAction();
    }
    #endregion
  }

}