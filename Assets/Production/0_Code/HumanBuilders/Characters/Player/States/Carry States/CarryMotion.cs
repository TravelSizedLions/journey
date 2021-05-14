using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// Super class that handles horizontal motion while carrying an object.
  /// </summary>
  public abstract class CarryMotion : MotionState {

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

      // Apply various motion settings.
      maxSpeed = settings.MaxCarrySpeed;
      groundJumpBuffer = settings.GroundJumpBuffer;
    }

    #endregion


    #region Motion State API

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

