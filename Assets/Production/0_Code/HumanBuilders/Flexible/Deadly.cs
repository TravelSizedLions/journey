using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// This behavior can be added to an object to signal that it should kill the player when touched.
  /// </summary>
  /// <remarks>
  /// This is specific to when the player is in their normal movement mode.
  /// </remarks>
  /// <seealso cref="PlayerCharacterOld" />
  /// <seealso cref="NormalMovement" />
  public class Deadly : MonoBehaviour {

    #region Fields
    /// <summary>
    /// The trigger collider.
    /// </summary>
    private Collider2D col;

    /// <summary>
    /// The set of layers to check when checking if the collider overlaps anything.
    /// </summary>
    private ContactFilter2D filter;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      col = GetComponent<Collider2D>();
      filter = new ContactFilter2D();
      filter.layerMask = LayerMask.GetMask("Player");
      filter.useLayerMask = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if (enabled && other.CompareTag("Player")) {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        player.Die();
      }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (enabled && collision.otherCollider.CompareTag("Player")) {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        player.Die();
      }
    }

    private void OnEnable() {
      if (col != null) {
        List<Collider2D> hits = new List<Collider2D>();
        if (Physics2D.OverlapCollider(col, filter, hits) > 0) {
          foreach (Collider2D hit in hits) {
            PlayerCharacter player = hit.GetComponent<PlayerCharacter>();
            if (player != null) {
              player.Die();
            }
          }
        }
      }
    }
    #endregion
  }


}