using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Storm.Characters.Player {

  public abstract class CarryBehavior : PlayerBehavior {
    protected void ChangeState<State>() where State: CarryBehavior {
      player.OnStateChange(this, gameObject.AddComponent<State>());
    }
  }
}