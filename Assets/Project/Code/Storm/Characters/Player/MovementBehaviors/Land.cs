using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Land : MovementBehavior {

    private HorizontalMotion motion;

    private void Awake() {
      AnimParam = "land";
      motion = GetComponent<HorizontalMotion>();
    }

    public void OnAnimationFinished() {
      ChangeState<Idle>();
    }

    public override void HandleInput() {
      if (Input.GetButton("Down")) {
        ChangeState<StartCrouch>();
      }
    }

    public override void HandlePhysics() {
      Facing facing = motion.Handle();
      player.SetFacing(facing);
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      // Clear up the stack.
      TryConsume(MovementSymbol.DoubleJumped);
      TryConsume(MovementSymbol.Jumped);
    }
  }
}
