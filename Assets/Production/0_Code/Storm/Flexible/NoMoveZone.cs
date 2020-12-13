using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// This behavior disables moving for the player character while they are within the collision area of the game object.
  /// </summary>
  /// <seealso cref="NoJumpZone" />
  public class NoMoveZone : MonoBehaviour {

    #region Fields
    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private PlayerCharacter player;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    
    private void OnEnable() {
      BoxCollider2D col = GetComponent<BoxCollider2D>();

      Collider2D[] hits = Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size, 0);
      foreach (var hit in hits) {
        if (hit.CompareTag("Player")) {
          player = hit.GetComponent<PlayerCharacter>();
          player.DisableMove(this);
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        player = other.GetComponent<PlayerCharacter>();
        player.DisableMove(this);
      }
    }

    private void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().EnableMove(this);
      }
    }

    private void OnDestroy() {
      if (player != null) {
        player.EnableMove(this);
      }
    }


    #endregion
  }
}