using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player stands up from crouch.
  /// </summary>
  public class CrouchEnd : PlayerState {

    #region Unity API
    private void Awake() {
      AnimParam = "crouch_end";
    }

    #endregion

    #region Player State API
    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      physics.Velocity = Vector2.zero;
    }


    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.TryingToMove()) {
        ChangeToState<Running>();
      } else if (player.HoldingDown()) {
        ChangeToState<CrouchStart>();
      } else if (player.HoldingJump()) {
        ChangeToState<SingleJumpStart>();
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
    /// Animation event hook.
    /// </summary>
    public void OnCrouchEndFinished() {
      ChangeToState<Idle>();
    }
    #endregion

  }
}