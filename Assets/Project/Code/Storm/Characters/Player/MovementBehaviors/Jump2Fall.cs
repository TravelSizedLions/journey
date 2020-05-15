using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public class Jump2Fall : HorizontalMotion {
    
    private void Awake() {
      AnimParam = "jump_2_fall";
    }
    
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsTouchingGround()) {
        ChangeToState<Land>();
      }
    }
  }

}