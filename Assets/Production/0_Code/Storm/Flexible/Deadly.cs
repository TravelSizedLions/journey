using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.Flexible {

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

    private bool playerInArea;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other) {
      if (enabled && other.CompareTag("Player")) {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        player.Die();
      }
    }

    private void OnTriggerStay2D(Collider2D other) {
      playerInArea = other.CompareTag("Player");
    }

    private void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        playerInArea = false;
      }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (enabled && collision.otherCollider.CompareTag("Player")) {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        player.Die();
      }
    }

    private void OnEnable() {
      if (playerInArea) {
        PlayerCharacter player = GameManager.Instance.player;
        player.Die();
        playerInArea = false;
      }
    }


    private void OnDisable() {
      playerInArea = false;
    }
    #endregion
  }


}