using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Storm.Characters.Player {

  public abstract class ConversationBehavior : PlayerBehavior {


    protected void ChangeState<State>() where State : ConversationBehavior {
      player.OnStateChange(this, gameObject.AddComponent<State>());
    }
  }
}