using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Subsystems.Transitions {
  /// <summary>
  /// A stupid class to make unity do what I want with the animation system.
  /// </summary>
  public class FadeToBlack : TransitionEffect {

    /// <summary>
    /// Animation event callback.
    /// </summary>
    public void OnFadedIn() {

    }

    /// <summary>
    /// Animation event callback.
    /// </summary>
    public void OnFadedOut() {
      TransitionManager.OnTransitionComplete();
    }
  }
}