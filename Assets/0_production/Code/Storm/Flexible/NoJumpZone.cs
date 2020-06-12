using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;


namespace Storm.Flexible {


  /// <summary>
  /// This behavior disables jumping for the player character while they are within the collision area of the game object.
  /// </summary>
  /// <seealso cref="NoMoveZone" />
  public class NoJumpZone : MonoBehaviour {

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().DisableJump();
      }
    }

    private void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().EnableJump();
      }
    }
    #endregion
  }
}