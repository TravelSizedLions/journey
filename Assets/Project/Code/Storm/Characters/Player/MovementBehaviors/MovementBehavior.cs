
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  public abstract class MovementBehavior : PlayerBehavior {
    public virtual void HandlePhysics() {
      
    }
    
    /// <summary>
    /// Change state. The old state behavior will be detached from the player after this call.
    /// </summary>
    protected void ChangeState<State>() where State : MovementBehavior {
      player.OnStateChange(this, player.gameObject.AddComponent<State>());
    }
  }
}