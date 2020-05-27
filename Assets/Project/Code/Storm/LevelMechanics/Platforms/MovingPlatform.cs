using System.Collections;
using System.Collections.Generic;
using Storm.Cameras;
using Storm.Characters.PlayerOld;
using UnityEngine;

namespace Storm.LevelMechanics.Platforms {

  /// <summary>
  /// Behavior for Moving Platforms. While the player is on the platform, they need to be added as a child transform.
  /// </summary>
  public class MovingPlatform : MonoBehaviour {

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    /// <summary>
    /// When the player touches the platform, set their parent transform to the platform.
    /// </summary>
    /// <param name="collision">Information about the collision that occured.</param>
    public void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player")) {
        
        Collider2D playerCol = collision.collider;

        BoxCollider2D col = GetComponent<BoxCollider2D>();

        // Make sure the collision is that the player is standing on top of the platform.
        float platformTop = transform.position.y + col.bounds.extents.y;
        float playerBottom = playerCol.bounds.center.y - playerCol.bounds.extents.y;

        // May not be *exactly* the same y coordinate.
        if (Mathf.Abs(playerBottom - platformTop) < 0.1) {
          collision.collider.transform.SetParent(transform);
          
          collision.collider.GetComponent<NormalMovement>().EnablePlatformMomentum();
        }
      }
    }

    /// <summary>
    /// Notify the player that they're no longer on the platform.
    /// </summary>
    /// <param name="collision">Information about the collision that occurred.</param>
    public void OnCollisionExit2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player")) {
        PlayerCharacterOld player = collision.collider.GetComponent<PlayerCharacterOld>();

        player.NormalMovement.DisablePlatformMomentum();
      }
    }

    #endregion
  }
}