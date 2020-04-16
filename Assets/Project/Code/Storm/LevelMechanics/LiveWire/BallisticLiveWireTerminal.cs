using Storm.Characters.Player;
using UnityEngine;

namespace Storm.LevelMechanics.LiveWire {

  /// <summary>
  /// A Level Object that transforms Jerrod into a spark of energy
  /// and allows the player to fling him in any direction in a ballistic arc.
  /// </summary>
  public class BallisticLiveWireTerminal : MonoBehaviour {

    /// <summary>
    /// Turns Jerrod into a spark of energy.
    ///
    /// This method fires when:
    /// 1. Another game object's collider component intersects with this game object's collider
    /// 2. This game object's collider is marked as "IsTrigger" in the inspector
    /// </summary>
    public void OnTriggerEnter2D(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        PlayerCharacter player = collider.gameObject.GetComponent<PlayerCharacter>();

        player.SwitchBehavior(PlayerBehaviorEnum.AimLiveWire);
        Debug.Log("Terminal: Activating livewire at " + transform.position);
        player.AimLiveWireMovement.SetLaunchPosition(transform.position);
      }
    }
  }

}