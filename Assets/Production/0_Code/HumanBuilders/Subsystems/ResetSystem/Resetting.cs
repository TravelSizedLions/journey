using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A base class that allows a behavior to be reset remotely.
  /// </summary>
  public abstract class Resetting : MonoBehaviour {

    #region Resetting API
    //-------------------------------------------------------------------------
    // Resetting API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Implement this function to have your object reset in the appropriate way.
    /// </summary>
    public abstract void ResetValues();
    #endregion
  }
}