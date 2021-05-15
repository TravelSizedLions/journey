using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// Shared behavior for player states that allow the player to move left/right.
  /// </summary>
  public abstract class MotionState : PlayerState {

    #region Fields
    /// <summary>
    /// The player's maximum horizontal movement speed.
    /// </summary>
    protected float maxSpeed;

    /// <summary>
    /// How quickly the player decelerates.
    /// </summary>
    protected float deceleration;

    /// <summary>
    /// The deceleration of the player in terms of a force vector.
    /// </summary>
    protected Vector2 decelerationForce;

    /// <summary>
    /// How quickly the player turns around while running.
    /// </summary>
    protected float agility;
    #endregion

    // [SerializeField]
    // [ReadOnly]
    // protected static float normalizedSpeed;

    [SerializeField]
    [ReadOnly]
    private float adjustedInput;


    #region Player State API
    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      deceleration = settings.Deceleration;
      decelerationForce = new Vector2(1-deceleration, 1);

      agility = settings.Agility;
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Tries to perform some kind of jump, accounting for any input leniency.
    /// </summary>
    public abstract bool TryBufferedJump();

    /// <summary>
    /// Translate user input into horizontal motion.
    /// </summary>
    /// <returns>Which direction the player should be facing.</returns>
    public Facing MoveHorizontally() {
      float input = player.GetHorizontalInput();
      bool movingEnabled = player.CanMove();

      float normalizedSpeed = physics.Vx/settings.MaxSpeed;

      if (!movingEnabled) {
        normalizedSpeed = HandleDeceleration(normalizedSpeed);
        physics.Vx = normalizedSpeed*settings.MaxSpeed;
        return GetFacing();
      } 

      TryUnparentPlayerTransform(player.IsPlatformMomentumEnabled(), input);


      // factor in turn around time.
      float inputDirection = Mathf.Sign(input);
      float motionDirection = Mathf.Sign(physics.Vx);
      adjustedInput = (inputDirection == motionDirection) ? (input) : (input*settings.Agility);

      if (Mathf.Abs(input) == 1 && player.CanInterruptWallJump()) {
        player.StopWallJumpMuting();
      }

      if (player.IsWallJumping()) {
        normalizedSpeed = HandleWallJumpMotion(input, normalizedSpeed);
      } else if ((Mathf.Abs(input) < 1e-4)) {
        normalizedSpeed = HandleDeceleration(normalizedSpeed);
      } else { 
        normalizedSpeed = HandleAcceleration(adjustedInput, normalizedSpeed);
      }

      float horizSpeed = normalizedSpeed*settings.MaxSpeed;
      physics.Vx = horizSpeed;

      return GetFacing();
    }

    private float HandleWallJumpMotion(float input, float normalizedSpeed) {
      if (Mathf.Abs(input) < 1e-4) {
        if (player.IsTouchingGround() && (Mathf.Abs(physics.Vx) < settings.IdleThreshold)) {
          normalizedSpeed = 0;
        } else {
          normalizedSpeed = Mathf.Sign(physics.Vx);
        }
      } else {
        normalizedSpeed = Mathf.Clamp(normalizedSpeed + input*settings.WallJumpMuting, -1, 1);
      }
      
      return normalizedSpeed;
    }

    private float HandleDeceleration(float normalizedSpeed) {
        float sign = Mathf.Sign(normalizedSpeed);
        float absSpeed = Mathf.Abs(normalizedSpeed);
        absSpeed = Mathf.Clamp(absSpeed-settings.Deceleration, 0, 1);
        normalizedSpeed = absSpeed*sign;

        return normalizedSpeed;
    }

    private float HandleAcceleration(float adjustedInput, float normalizedSpeed) {
      return Mathf.Clamp(normalizedSpeed + (settings.Acceleration*adjustedInput), -1, 1);
    }


    /// <summary>
    /// Attempt to remove the Player's transform from a platform's list of child
    /// transforms.
    /// </summary>
    /// <param name="platformMomentumEnabled">Whether or not platform momentum
    /// is currently enabled for the player.</param>
    /// <param name="input">The player's horizontal axis input.</param>
    /// <returns>Whether or not the player's transform was freed from its parent
    /// transform. True if yes, false if no.</returns>
    public bool TryUnparentPlayerTransform(bool platformMomentumEnabled, float input) {
      // Prevents the player from being dragged around by a platform they 
      // may have been parented to.
      if (!platformMomentumEnabled && input != 0) {
        transform.SetParent(null);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Attempt to decelerate the player.
    /// </summary>
    /// <param name="input">The player's horizontal axis input.</param>
    /// <param name="wallJumping">Whether or not the player is wall currently in
    /// the air from a wall jump.</param>
    /// <param name="movingEnabled">Whether or not moving is enabled.</param>
    /// <returns>True if the player was decelerated. False otherwise.</returns>
    public bool TryDecelerate(float input, bool wallJumping, bool movingEnabled) {      
      if ((Mathf.Abs(input) != 1 && !wallJumping) || (!movingEnabled && !wallJumping)) {
        physics.Velocity *= decelerationForce;
        return true;
      }

      return false;
    }

    /// <summary>
    /// Get which direction the player is supposed to face based on their
    /// horizontal velocity.
    /// </summary>
    /// <returns>Which direction the player should face.</returns>
    public Facing GetFacing() {
      if (Mathf.Abs(physics.Vx) < settings.IdleThreshold) {
        return Facing.None;
      } else {
        return (Facing)Mathf.Sign(physics.Vx);
      }
    }
    #endregion
  }
}