using System.Collections;
using System.Collections.Generic;
using Storm.Extensions;
using UnityEngine;

namespace Storm.ResetSystem {

  /// <summary>
  /// A management system that allows you to reset any object
  /// that inherits from the Resetting base class.
  /// </summary>
  /// <seealso cref="Resetting" />
  /// <seealso cref="CrumblingPlatform" />
  /// <seealso cref="DoorKey" />
  /// <seealso cref="Moving" />
  public class ResetManager : Singleton<ResetManager> {

    /// <summary>
    /// Reset every resettable object in the current level.
    /// </summary>
    public void Reset() {
      foreach (var r in GameObject.FindObjectsOfType<Resetting>()) {
        r.Reset();
      }
    }
  }
}