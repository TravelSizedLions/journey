using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using Storm.Flexible.Interaction;
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
      if (player.PressedAction()) {
        Debug.Log("Interacting!");
        player.Interact();
      } else if (player.PressedJump()) {
        if (player.IsTouchingRightWall() || player.IsTouchingLeftWall()) {
          ChangeToState<WallRun>();
        } else {
          ChangeToState<SingleJumpStart>();
        }
      } else if (player.HoldingDown()) {
        ChangeToState<CrouchStart>();
      } else if (player.TryingToMove()) {
        ChangeToState<Running>();
      }
    }


    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      if (!player.IsTouchingGround()) {
        ChangeToState<SingleJumpFall>();
      }
    }
    
    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      physics.Velocity = Vector2.zero;
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<PickUpItem>();
      }
    }
    #endregion
  }

}