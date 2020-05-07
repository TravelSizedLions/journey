using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Land : MovementBehavior {
    public void OnAnimationFinished() {
      player.TriggerAnimation();
      ChangeState<Idle>();
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      player.SetAnimParam("in_air", false);
      player.SetAnimParam("y_velocity", p.GetComponent<Rigidbody2D>().velocity.y);
    }
  }
}