using Storm.LevelMechanics.Platforms;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// The callback used to determine where the amount of time the coyote time
  /// needs should come from.
  /// </summary>
  public delegate float TimerSource();

  /// <summary>
  /// Interface for the Coyote Time component.
  /// </summary>
  public interface ICoyoteTime {

    /// <summary>
    /// Callback for where to get the amount of time for the timer.
    /// </summary>
    event TimerSource CoyoteTime;

    /// <summary>
    /// Advance coyote time by one physics tick.
    /// </summary>
    void Tick();

    /// <summary>
    /// Reset the coyote timer.
    /// </summary>
    void StartCoyoteTime();

    /// <summary>
    /// Whether or not the player can still register a jump input.
    /// </summary>
    /// <returns>True if the player is still close enough to the ledge to
    /// register a jump. False otherwise.</returns>
    bool InCoyoteTime();

    /// <summary>
    /// Use up the remaining coyote time to perform a junmp.
    /// </summary>
    void UseCoyoteTime();
  }


  /// <summary>
  /// A class for keeping track of <see href="https://www.gamasutra.com/blogs/LisaBrown/20171005/307063/GameMaker_Platformer_Jumping_Tips.php">Coyote Time</see> for the player.
  /// </summary>
  public class CoyoteTimer : MonoBehaviour, ICoyoteTime {

    #region Fields
    /// <summary>
    /// Callback for where to get the amount of time for the timer. This allows
    /// </summary>
    public event TimerSource CoyoteTime;

    /// <summary>
    /// How much time the player has left to input a single jump after leaving a ledge.
    /// </summary>
    private float timer;
    #endregion

    #region  Unity API
    private void FixedUpdate() {
      Tick();
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Advance coyote time by one physics tick.
    /// </summary>
    public void Tick() {
      timer += Time.fixedDeltaTime;
    }

    /// <summary>
    /// Reset the coyote timer.
    /// </summary>
    public void StartCoyoteTime() {
      timer = 0;
    }

    /// <summary>
    /// Whether or not the player can still register a jump input.
    /// </summary>
    /// <returns>True if the player is still close enough to the ledge to
    /// register a jump. False otherwise.</returns>
    public bool InCoyoteTime() {
      return timer < CoyoteTime();
    }

    /// <summary>
    /// Use up the remaining coyote time to perform a junmp.
    /// </summary>
    public void UseCoyoteTime() {
      timer = CoyoteTime();
    }
    #endregion
  }
}