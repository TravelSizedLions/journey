using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A ghost in platforming levels that whispers mean things to the player.
  /// </summary>
  public class Ghost : Resetting {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The animator that displays the dialog for this ghost.
    /// </summary>
    private Animator anim;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    void Start() {
      anim = GetComponentInChildren<Animator>();
    }

    #endregion



    #region Resetting API
    //-------------------------------------------------------------------------
    // Resetting API
    //-------------------------------------------------------------------------
    public override void ResetValues() {
      if (anim == null) {
        anim = GetComponentInChildren<Animator>();       
      }
      
      anim.SetTrigger("reset");
    }
    #endregion
  }
}