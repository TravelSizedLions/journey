using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Components;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player is crawling on the floor.
  /// </summary>
  [RequireComponent(typeof(MovementSettings))]
  public class Crawling : PlayerState {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "crawling";

    /// <summary>
    /// The speed at which the player crawls.
    /// </summary>
    private float crawlSpeed;
    #endregion

    #region Player State API

    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      MovementSettings settings = GetComponent<MovementSettings>();
      crawlSpeed = settings.CrawlSpeed;
    }

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (!player.HoldingDown() && player.TryingToMove() && player.DistanceToCeiling() > 1f) {
        ChangeToState<Running>();
      }
    }
    
    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {

      if (!player.IsTouchingGround()) {
        player.StartCoyoteTime();
        ChangeToState<SingleJumpFall>();
      }

      if (!player.CanMove()) {
        return;
      }

      float input = player.GetHorizontalInput();

      if (input != 0) {
        float inputDirection = Mathf.Sign(input);
        float playerMovement = inputDirection*crawlSpeed;

        physics.Vx = playerMovement;

        Facing facing = (Facing)inputDirection;
        player.SetFacing(facing);
      } else {
        physics.Vx = 0;
        if (player.DistanceToCeiling() < 1f) {
          
          ChangeToState<CrawlingStopped>();
        } else {
          ChangeToState<Crouching>();
        }
        
      }

    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }


    public override void OnStateEnter() {
      bool left = player.IsTouchingLeftWall();
      bool right = player.IsTouchingRightWall();


      // The player interacts strangely when trying to start crawling while
      // already next to a wall. Keeps the player's collider from clipping into
      // the wall.
      if (left && !right) {
        float dist = player.DistanceToLeftWall();
        if (dist != float.PositiveInfinity && dist != float.NegativeInfinity) {
          player.Physics.Px -= dist;
        }
      }

      if (right && !left) {
        float dist = player.DistanceToRightWall();
        if (dist != float.PositiveInfinity && dist != float.NegativeInfinity) {
          player.Physics.Px += dist;
        }
      }
    }
    #endregion


    #region Getters/Setters

    public void SetCrawlSpeed(float value) {
      crawlSpeed = value;
    }

    public float GetCrawlSpeed() {
      return crawlSpeed;
    }
    #endregion
  }
}