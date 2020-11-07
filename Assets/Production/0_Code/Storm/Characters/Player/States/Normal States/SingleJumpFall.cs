using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is falling after their first jump.
  /// </summary>
  public class SingleJumpFall : HorizontalMotion {

    #region Fields
    /// <summary>
    /// The amount of time the player needs to be falling to turn the landing into a roll.
    /// </summary>
    private float rollOnLand;

    /// <summary>
    /// How long the player is allowed to fall before a jump counts as a double jump rather than a single jump.
    /// </summary>
    private float coyoteTime;

    /// <summary>
    /// Times how long the player has been falling in this state.
    /// </summary>
    private float fallTimer;


    /// <summary>
    /// The maximum speed the player can fall.
    /// </summary>
    private float maxFallSpeed;


    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "jump_1_fall";
    }
    #endregion


    #region Player State API

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (player.InCoyoteTime()) {
          player.UseCoyoteTime();
          ChangeToState<SingleJumpStart>();
        } else if (player.InWallCoyoteTime()) {
          player.UseWallCoyoteTime();
          ChangeToState<WallJump>();
        } else if(!base.TryBufferedJump()) {
          ChangeToState<DoubleJumpStart>();
        } else if (player.PressedAction() || player.PressedAltAction()) {
          player.Interact();
        }
      }
    }

    #region OnFixedUpdate() logic stack.
    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      fallTimer += Time.fixedDeltaTime;

      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.Physics.Vy < -settings.MaxFallSpeed) {
        player.Physics.Vy = -settings.MaxFallSpeed;
      }

      if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        ChangeToState<WallSlide>();
      } else if (player.IsTouchingGround()) {
        if (Mathf.Abs(physics.Vx) > idleThreshold) {
          PickLanding<RollStart, CrouchStart, Land>();
        } else {
          PickLanding<CrouchEnd, CrouchStart, Land>();
        }
      }
    }

    /// <summary>
    /// Pick between one of three landing types.
    /// </summary>
    /// <typeparam name="RollTimeState">The landing to use when the player has been falling long enough to roll. </typeparam>
    /// <typeparam name="CrouchState">The landing to use if the player is trying to crouch.</typeparam>
    /// <typeparam name="LandState">The default landing state.</typeparam>
    private void PickLanding<RollTimeState,CrouchState,LandState>() 
      where RollTimeState : PlayerState 
      where CrouchState : PlayerState 
      where LandState : PlayerState {

      if (fallTimer > rollOnLand) {
        ChangeToState<RollTimeState>();
      } else  {
        if (player.HoldingDown()) {
          ChangeToState<CrouchState>();
        } else {
          ChangeToState<LandState>();
        }
      }
    }
    #endregion

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
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      fallTimer = 0;
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        Carriable item = obj.GetComponent<Carriable>();
        item.OnPickup();
        ChangeToState<CarryJumpFall>();
      } else if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      }
    }
    #endregion
  }
}