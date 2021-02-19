using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A door that can be destroyed by throwing an object at it.
  /// </summary>
  public class DestructibleDoor : PuzzleElement {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The sprite for the door.
    /// </summary>
    private SpriteRenderer sprite;

    /// <summary>
    /// Box collider for the door.
    /// </summary>
    private BoxCollider2D col;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      sprite = GetComponent<SpriteRenderer>();
      col = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D col) {
      Transform root = col.collider.transform.root;
      Carriable carriable = root.GetComponentInChildren<Carriable>();
      if (carriable != null) {
        carriable.Physics.Velocity = Vector2.zero;
        Solve();
      }
    }

    #endregion

    #region Resetting API
    //-------------------------------------------------------------------------
    // Resetting API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Re-enable the door.
    /// </summary>
    protected override void OnResetElement() {
      col.enabled = true;
      sprite.enabled = true;
    }

    /// <summary>
    /// Destroys the door.
    /// </summary>
    protected override void OnSolved() {
      col.enabled = false;
      sprite.enabled = false;
    }

    #endregion

  }
}