﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// When the player is sliding down a wall.
  /// </summary>
  public class WallSlideFast : HorizontalMotion {

    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "wall_slide_fast";

    /// <summary>
    /// Which wall the player is sliding down (left or right).
    /// </summary>
    private Facing whichWall;

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (!player.HoldingDown()) {
        ChangeToState<WallSlide>();
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
        //transform.position = new Vector3(transform.position.x, transform.position.y-0.2f, transform.position.z);
        ChangeToState<SingleJumpFall>();
        return;
      } else if (player.IsTouchingGround()) {
        ChangeToState<Idle>();
        return;
      } else {
        float input = player.GetHorizontalInput();

        if ((leftWall && input < 0) || (rightWall && input > 0)) {
          physics.Vx = 0;
          physics.Vy *= (1 - settings.FastWallSlideDeceleration);
        } else {
          physics.Vy *= (1 - settings.FastWallSlideDeceleration);
        }
      }
    }

    private void NudgePlayer() {
      float nudge = 0.5f;
      player.Physics.Px -= ((int)whichWall)*nudge; 
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      whichWall = ProjectToWall();
      player.SetFacing((Facing)(-(int)whichWall));
    }
    
    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      base.OnStateAdded();
      MovementSettings settings = GetComponent<MovementSettings>();
    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }
  }
}