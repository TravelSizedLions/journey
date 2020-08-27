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

      try {
        Vector3 position = TransitionManager.Instance.GetCurrentSpawnPosition();
        if (position != Vector3.positiveInfinity) {
          player.Physics.Position = position;
        }
        
        player.Physics.Velocity = Vector2.zero;
        
        bool facingRight = TransitionManager.Instance.GetCurrentSpawnFacing();
        Facing facing = facingRight ? Facing.Right : Facing.Left;
        player.SetFacing(facing);
      } catch (UnityException e) {
        Debug.Log(e);
      }

    }

    #endregion
  }
}