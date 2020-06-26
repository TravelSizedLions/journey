using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// A behavior that allows a game object to be destroyed on command. This is useful when a game object needs to be destroyed through an event trigger.
  /// </summary>
  public class SelfDestructing : MonoBehaviour {

    #region Fields

    /// <summary>
    /// How long to wait before self destructing (in seconds).
    /// </summary>
    [Tooltip("How long to wait before self destructing (in seconds).")]
    public float Delay = 0f;

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Destroy this object.
    /// </summary>
    public void SelfDestruct() {
      Destroy(this.gameObject, Delay);
    }
    #endregion
  }
}