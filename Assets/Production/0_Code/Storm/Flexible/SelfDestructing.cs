using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// A behavior that allows a game object to be destroyed on command. This is useful when a game object needs to be destroyed through an event trigger.
  /// </summary>
  public class SelfDestructing : MonoBehaviour {
    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Destroy this object.
    /// </summary>
    public void SelfDestruct() {
      Destroy(this.gameObject);
    }

    #endregion
  }
}