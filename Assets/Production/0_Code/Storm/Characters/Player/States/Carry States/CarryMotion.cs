using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// Super class that handles horizontal motion while carrying an object.
  /// </summary>
  public class CarryMotion : MotionState {
    #region Fields
    /// <summary>
    /// How close the player has to be to the ground in order to register another jump.
    /// </summary>
    private float groundJumpBuffer;

    #endregion

    #region Player State API
    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      base.OnStateAdded();

      CarrySettings carrySettings = GetComponent<CarrySettings>();
      MovementSettings motionSettings = GetComponent<MovementSettings>();

      // Apply various motion settings.
      maxSpeed = carrySettings.MaxCarrySpeed;
      maxSqrVelocity = maxSpeed*maxSpeed;
      accelerationFactor = maxSpeed*acceleration;
      groundJumpBuffer = motionSettings.GroundJumpBuffer;
    }

    #endregion


    #region Motion State API

    /// <summary>
    /// Translate user input into horizontal motion.
    /// </summary>
    /// <returns>Which direction the player should be facing.</returns>
    public override Facing MoveHorizontally() {
      float input = player.GetHorizontalInput();
      bool movingEnabled = player.CanMove();

      TryDecelerate(input, false, movingEnabled, player.IsTouchingGround());

      if (!movingEnabled) {
        return GetFacing();
      }

      TryUnparentPlayerTransform(player.IsPlatformMomentumEnabled(), input);

      // factor in turn around time.
      float inputDirection = Mathf.Sign(input);
      float motionDirection = Mathf.Sign(physics.Vx);
      float adjustedInput = (inputDirection == motionDirection) ? (input) : (input*agility);
      

      float horizSpeed = Mathf.Clamp(physics.Vx + (adjustedInput*accelerationFactor), -maxSpeed, maxSpeed);
      physics.Vx = horizSpeed;
      
      return GetFacing();
    }

    /// <summary>
    /// Tries to perform a jump, accounting for input leniency.
    /// </summary>
    public override bool TryBufferedJump() {
      float distToFloor = player.DistanceToGround();

      if (distToFloor < groundJumpBuffer) {
        ChangeToState<CarryJumpStart>();
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}

