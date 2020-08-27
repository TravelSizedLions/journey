using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is sliding down a wall.
  /// </summary>
  public class WallSlide : HorizontalMotion {

    #region Fields
    private Facing whichWall;
    #endregion

    #region Unity API
    private void Awake() {
      AnimParam = "wall_slide";
    }
    #endregion


    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.HoldingDown()) {
        ChangeToState<WallSlideFast>();
      } else if (player.PressedJump()) {
        ChangeToState<WallJump>();
      } 
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {  
      Facing facing = MoveHorizontally();
      bool isTouching;
      if (whichWall == Facing.Left) {
        isTouching = player.IsTouchingLeftWall();
      } else {
        isTouching = player.IsTouchingRightWall();
      }

      bool leftWall = player.IsTouchingLeftWall();
      bool rightWall = player.IsTouchingRightWall();

      if (!isTouching) {
        NudgePlayer();
        ChangeToState<SingleJumpFall>();
        return;
      } else if (player.IsTouchingGround()) {
        ChangeToState<Idle>();
        return;
      } else {
        float input = player.GetHorizontalInput();
        if ((leftWall && input < 0) || (rightWall && input > 0)) {
          physics.Vx = 0;
          physics.Vy *= (1 - settings.WallSlideDeceleration);
        } else {
          physics.Vy *= (1 - settings.WallSlideDeceleration);
        }
      }
    }

    private void NudgePlayer() {
      float nudge = 0.08f;
      player.Physics.Px -= ((int)whichWall)*nudge; 
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      whichWall = ProjectToWall();
      player.SetFacing(whichWall);
    }
    
    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      base.OnStateAdded();
      MovementSettings settings = GetComponent<MovementSettings>();
    }
    #endregion

    #region Getters/Setters

    /// <summary>
    /// Set which wall the player is sliding down (left or right).
    /// </summary>
    /// <param name="facing">The direction of the wall relative to the player.</param>
    public void SetWallFacing(Facing facing) {
      whichWall = facing;
    }
    #endregion
  }
}