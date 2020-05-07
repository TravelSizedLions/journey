using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class Jump : MovementBehavior {
    public void OnAnimationFinished() {
      ChangeState<Ascend>();
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);
      Debug.Log("Jump Start!");
    }
  }
}