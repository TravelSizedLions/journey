using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A door that can be destroyed by throwing an object at it.
  /// </summary>
  public class DestructibleDoor : PuzzleElement {

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
    private Collider2D col;


    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected void Awake() {
      sprite = GetComponent<SpriteRenderer>();
      col = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D col) {
      if (IsSolved(col)) {
        Transform root = col.collider.transform.root;
        Carriable carriable = root.GetComponentInChildren<Carriable>();
        carriable.Physics.Velocity = Vector2.zero;
        Solve();
      }
    }

    //-------------------------------------------------------------------------
    // Resetting API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Re-enable the door.
    /// </summary>
    protected override void OnResetElement() {
      col.enabled = true;
      sprite.enabled = true;
      foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) {
        c.enabled = true;
      }
    }

    //-------------------------------------------------------------------------
    // Puzzle API
    //-------------------------------------------------------------------------

    protected override bool IsSolved(object info) {
      if (info is Collision2D col) {
        Transform root = col.collider.transform.root;
        Carriable carriable = root.GetComponentInChildren<Carriable>();

        return carriable != null && carriable != GameManager.Player.CarriedItem;
      }

      return false;
    }

    /// <summary>
    /// Destroys the door.
    /// </summary>
    protected override void OnSolved() {
      col.enabled = false;
      sprite.enabled = false;
      foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) {
        c.enabled = false;
      }
    }

  }
}