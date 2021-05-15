using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// Super class that handles horizontal motion while carrying an object.
  /// </summary>
  public abstract class CarryMotion : MotionState {
    public override bool TryBufferedJump() {
      float distToFloor = player.DistanceToGround();

      if (distToFloor < settings.GroundJumpBuffer) {
        ChangeToState<CarryJumpStart>();
        return true;
      } else {
        return false;
      }
    }
  }
}

