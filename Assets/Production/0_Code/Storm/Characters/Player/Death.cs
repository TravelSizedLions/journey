using UnityEngine;

using Storm;
using Storm.Subsystems.Transitions;

namespace Storm.Characters.Player {

  /// <summary>
  /// A component for killing the player.
  /// </summary>
  public class Death : MonoBehaviour {

    private PlayerCharacter player;


    private void Awake() {
      this.player = GetComponent<PlayerCharacter>();
    }


    #region Public API

    /// <summary>
    /// Kill the player.
    /// </summary>
    public void Die() {
      Instantiate(
        player.EffectsSettings.DeathEffect,
        player.Physics.Position,
        Quaternion.identity
      );
      
      GameManager.Instance.resets.Reset();
      player.Physics.Position = TransitionManager.Instance.GetCurrentSpawnPosition();
      player.Physics.Velocity = Vector2.zero;
      

    }

    #endregion
  }
}