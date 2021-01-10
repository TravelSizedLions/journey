using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HumanBuilders {
  /// <summary>
  /// A stupid class to make unity do what I want with the animation system.
  /// </summary>
  public class Wipe : TransitionEffect {

    /// <summary>
    /// Animation event callback.
    /// </summary>
    public void OnWiping() {
      GameManager.Player.Respawn();
    }
  }
}