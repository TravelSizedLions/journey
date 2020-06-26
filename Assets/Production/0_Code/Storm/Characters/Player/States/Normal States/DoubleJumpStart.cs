using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {

  /// <summary>
  /// When the player prepares to do a double jump.
  /// </summary>
  public class DoubleJumpStart : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "jump_2_start";
    }
    #endregion

    #region State API
    
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (player.InCoyoteTime()) {
          player.UseCoyoteTime();
          ChangeToState<SingleJumpStart>();
        } else {
          base.TryBufferedJump();
        }
      } else if (player.PressedAction()) {
        player.Interact();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      if (player.IsTouchingGround()) {

        if (Mathf.Abs(physics.Vx) > idleThreshold) {
          ChangeToState<RollStart>();
        } else {
          ChangeToState<Land>();
        }
      } else if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        ChangeToState<WallSlide>();
      }
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      MovementSettings settings = GetComponent<MovementSettings>();
      physics.Vy = settings.DoubleJumpForce;

      player.DisablePlatformMomentum();
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnDoubleJumpFinished() {
      if (player.IsRising()) {
        ChangeToState<DoubleJumpRise>();
      } else {  
        ChangeToState<DoubleJumpFall>();
      }
    }

    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<CarryJumpRise>();
      }
    }

    #endregion

  }

}