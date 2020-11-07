using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is falling from their second jump.
  /// </summary>
  public class DoubleJumpFall : HorizontalMotion {
    
    #region Fields
    /// <summary>
    /// The amount of time the player needs to be falling to turn the landing into a roll.
    /// </summary>
    private float rollOnLand;

    /// <summary>
    /// The maximum speed the player is allowed to fall.
    /// </summary>
    private float maxFallSpeed;

    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "jump_2_fall";
    }
    #endregion
    
    #region Player State API
    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      base.OnStateAdded();

      MovementSettings settings = GetComponent<MovementSettings>();
      rollOnLand = settings.RollOnLand;
      maxFallSpeed = settings.MaxFallSpeed;
    }

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        base.TryBufferedJump();
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

      if (player.Physics.Vy < -settings.MaxFallSpeed) {
        player.Physics.Vy = -settings.MaxFallSpeed;
      }

      if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        ChangeToState<WallSlide>();
      } else if (player.IsTouchingGround()) {
        if (Mathf.Abs(physics.Vx) > idleThreshold) {
          ChangeToState<RollStart>();
        } else {
          if (player.HoldingDown()) {
            ChangeToState<CrouchStart>();
          } else {
            ChangeToState<Land>();  
          }
        }
      } 
    }
    
    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<CarryJumpFall>();
      } else if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      }
    }

    #endregion
  }
}