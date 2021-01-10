using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// The base class for anything the player could collect in-game. Examples of this would be currency or in-game items like keys.
  /// </summary>
  [RequireComponent(typeof(SpriteRenderer))]
  [RequireComponent(typeof(Collider2D))]
  public abstract class Collectible : MonoBehaviour {

    #region Variables
    /// <summary>
    /// Whether or not the item has been collected.
    /// </summary>
    protected bool collected = false;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected virtual void Awake() {

    }

    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        OnCollected();
      }
    }

    #endregion


    #region Collectible API
    //-------------------------------------------------------------------------
    // Collectible API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Base method for collection behavior. Whatever you need your collectible to do, define it here.
    /// </summary>
    public virtual void OnCollected() {
      collected = true;
    }


    /// <summary>
    /// Whether or not the collectible has already been collected.
    /// </summary>
    public bool IsCollected() {
      return collected;
    }
    #endregion
  }
}