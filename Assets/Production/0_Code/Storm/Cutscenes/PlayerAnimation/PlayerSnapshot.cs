using Storm.Characters;
using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;

namespace Storm.Cutscenes {
  
  /// <summary>
  /// A snapshot of important visual infomation for the player character.
  /// </summary>
  public class PlayerSnapshot {
    /// <summary>
    /// A driver for the state that the player's finite state machine was
    /// previously in. This allows the finite state machine to roll back to
    /// its previous state.
    /// </summary>
    private StateDriver driver;

    /// <summary>
    /// The player's position.
    /// </summary>
    private Vector3 position;

    /// <summary>
    /// The player's euler rotation (x, y, z).
    /// </summary>
    private Vector3 rotation;

    /// <summary>
    /// The player's local scale.
    /// </summary>
    private Vector3 scale;

    /// <summary>
    /// Whether or not the player is facing left or right.
    /// </summary>
    private Facing facing;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="player">The player character.</param>
    public  PlayerSnapshot(PlayerCharacter player) {
      driver = StateDriver.For(player.FSM.CurrentState);
      position = player.transform.position;
      rotation = player.transform.eulerAngles;
      scale = player.transform.localScale;
      facing = player.Facing;
    }

    /// <summary>
    /// Restore the transform the player was in.
    /// </summary>
    /// <param name="player">The player character.</param>
    public void RestoreTransform(PlayerCharacter player) {
      player.transform.position = position;
      player.transform.eulerAngles = rotation;
      player.transform.localScale = scale;
      player.SetFacing(facing);
    }

    /// <summary>
    /// Restore the state the player's state machine was in.
    /// </summary>
    /// <param name="player">The player character.</param>
    public void RestoreState(PlayerCharacter player) {
      if (driver != null && !driver.IsInState(player.FSM)) {
        driver.ForceStateChangeOn(player.FSM);
      }
    }
  }
}