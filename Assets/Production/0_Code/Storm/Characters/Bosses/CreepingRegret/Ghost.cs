using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Reset;
using UnityEngine;

namespace Storm.Characters.Bosses {

  /// <summary>
  /// A ghost in platforming levels that whispers mean things to the player.
  /// </summary>
  public class Ghost : Resetting {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The animator that opens/closes this eye.
    /// </summary>
    private Animator anim;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    // Start is called before the first frame update
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