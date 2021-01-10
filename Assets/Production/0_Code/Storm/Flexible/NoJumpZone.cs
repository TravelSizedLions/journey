
using UnityEngine;


namespace HumanBuilders {


  /// <summary>
  /// This behavior disables jumping for the player character while they are within the collision area of the game object.
  /// </summary>
  /// <seealso cref="NoMoveZone" />
  public class NoJumpZone : MonoBehaviour {

    #region Fields
    /// <summary>
    /// A reference to the player
    /// </summary>
    private PlayerCharacter player;

    private ICollision collisionComponent;
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
          player.DisableJump(this);
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        player = other.GetComponent<PlayerCharacter>();
        player.DisableJump(this);
      }
    }

    private void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().EnableJump(this);
      }
    }

    private void OnDestroy() {
      if (player != null) {
        player.EnableJump(this);
      }
    }

    #endregion
  }
}