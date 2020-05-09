using System.Collections;
using System.Collections.Generic;
using Storm.Characters.PlayerOld;
using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// This behavior can be added to an object to signal that it should kill the player when touched.
  /// </summary>
  /// <remarks>
  /// This is specific to when the player is in their normal movement mode.
  /// </remarks>
  /// <seealso cref="PlayerCharacter" />
  /// <seealso cref="NormalMovement" />
  public class Deadly : MonoBehaviour {

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        player.SwitchBehavior(PlayerBehaviorEnum.Normal);
        GameManager.Instance.KillPlayer(player);
      }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
      if (collision.otherCollider.CompareTag("Player")) {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        player.SwitchBehavior(PlayerBehaviorEnum.Normal);
        GameManager.Instance.KillPlayer(player);
      }
    }
    #endregion
  }


}