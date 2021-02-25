using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A base class that allows a behavior to be reset remotely.
  /// </summary>
  public abstract class Resetting : MonoBehaviour {

    /// <summary>
    /// Whether or not automatic resetting is allowed.
    /// </summary>
    private bool allowReset = true;

    #region Resetting API
    //-------------------------------------------------------------------------
    // Resetting API
    //-------------------------------------------------------------------------

    public void EnableResetting() {
      allowReset = true;
    }

    public void DisableResetting() {
      allowReset = false;
    }


    public void Reset() {
      if (allowReset) {
        ResetValues();
      }
    }

    /// <summary>
    /// Implement this function to have your object reset in the appropriate way.
    /// </summary>
    public abstract void ResetValues();
    #endregion
  }
}