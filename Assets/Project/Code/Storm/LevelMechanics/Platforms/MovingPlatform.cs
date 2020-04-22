using System.Collections;
using System.Collections.Generic;
using Storm.Cameras;
using Storm.Characters.Player;
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
        collision.collider.transform.SetParent(transform);
        PlayerCharacter player = collision.collider.GetComponent<PlayerCharacter>();

        player.NormalMovement.EnablePlatformMomentum();
      }
    }

    /// <summary>
    /// Notify the player that they're no longer on the platform.
    /// </summary>
    /// <param name="collision">Information about the collision that occurred.</param>
    public void OnCollisionExit2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player")) {
        PlayerCharacter player = collision.collider.GetComponent<PlayerCharacter>();

        player.NormalMovement.DisablePlatformMomentum();
      }
    }

    #endregion
  }
}