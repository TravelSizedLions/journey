using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.LevelMechanics.LiveWire {
  /// <summary>
  /// The abstract base class for the Livewire system. This system allows the player
  /// to turn into a spark of energy and be shot across the level at high speed.
  /// </summary>  
  /// <seealso cref="LivewireTerminal" />
  /// <seealso cref="LivewireRedirect" />
  /// <seealso cref="BallisticLivewireTerminal" />
  /// <seealso cref="BallisticLivewireRedirect" />
  public abstract class LivewireNode : MonoBehaviour {

    #region Temporary Collision Disabling Variables

    /// <summary>
    /// A reference to the object's box collider.
    /// </summary>
    protected BoxCollider2D boxCollider;

    /// <summary>
    /// A timer to count down how long the collider should be disabled after redirecting the player.
    /// </summary>
    protected float disableTimer;
    
    /// <summary>
    /// How long the redirection node should be disabled to allow the player to escape its collision area.
    /// </summary>
    protected float delay = 0.125f;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected virtual void Awake() {
      boxCollider = GetComponent<BoxCollider2D>();
    }

    protected void Update() {
      // Keep the collider disabled for a while after launching the
      // character long enough for them to escape the collider area.
      if (!boxCollider.enabled) {
        disableTimer -= Time.deltaTime;
        if (disableTimer < 0) {
          boxCollider.enabled = true;
        }
      }
    }

    // Forces inheriting classes to overide this.
    protected abstract void OnTriggerEnter2D(Collider2D col);
  
    #endregion
  }
}
