

using UnityEngine;

namespace HumanBuilders {

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
    /// Find and reset all "resettable" objects in the scene.
    /// </summary>
    public static void Reset() => Instance.Reset_Inner();
    private void Reset_Inner() {
      foreach (var r in GameObject.FindObjectsOfType<Resetting>()) {
        r.Reset();
      }
    }
  }
}