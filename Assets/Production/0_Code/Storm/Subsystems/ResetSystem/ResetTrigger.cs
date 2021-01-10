using System.Collections.Generic;
using UnityEngine;


namespace HumanBuilders {

  /// <summary>
  /// A remote trigger for IResettables.
  /// </summary>
  public class ResetTrigger : Resetting {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The list of game objects to reset. These game objects must implement the
    /// interface IResettable.
    /// </summary>
    [Tooltip("The list of game objects to reset. These game objects must implement the interface IResettable.")]
    public List<GameObject> Resettables;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      if (Resettables == null) {
        Resettables = new List<GameObject>();
      }

    }
    #endregion

    #region Reset
    //-------------------------------------------------------------------------
    // Resetting API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Reset the boss.
    /// </summary>
    public override void ResetValues() {
      foreach (GameObject g in Resettables) {
        IResettable r = g.GetComponent<IResettable>();
        if (r != null) {
          r.Reset();
        }
      }
    }
    #endregion
  }
}