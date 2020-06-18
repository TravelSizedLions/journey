using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
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
    }

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        base.TryBufferedJump();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>  
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

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

    public override void OnSignal(GameObject obj) {
      Carriable carriable = obj.GetComponent<Carriable>();
      if (carriable != null) {
        carriable.OnPickup();
        ChangeToState<CarryJumpFall>();
      }
    }

    #endregion
  }
}