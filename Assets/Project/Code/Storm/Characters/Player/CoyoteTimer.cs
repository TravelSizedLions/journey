using Storm.LevelMechanics.Platforms;
using UnityEngine;

namespace Storm.Characters.Player {


  /// <summary>
  /// A class for keeping track of <see href="https://www.gamasutra.com/blogs/LisaBrown/20171005/307063/GameMaker_Platformer_Jumping_Tips.php">Coyote Time</see> for the player.
  /// </summary>
  public class CoyoteTimer : MonoBehaviour {

    #region Fields
    /// <summary>
    /// How much time the player has left to input a single jump after leaving a ledge.
    /// </summary>
    private float timer;

    /// <summary>
    /// How long the player can be off a ledge before they can't register a
    /// single jump.
    /// </summary>
    private float coyoteTime = 0;
    #endregion


    #region  Unity API
    private void Start() {
      MovementSettings settings = GetComponent<MovementSettings>();
      coyoteTime = settings.CoyoteTime;
    }

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
    public void Reset() {
      timer = 0;
    }

    /// <summary>
    /// Whether or not the player can still register a jump input.
    /// </summary>
    /// <returns>True if the player is still close enough to the ledge to
    /// register a jump. False otherwise.</returns>
    public bool InCoyoteTime() {
      return timer < coyoteTime;
    }

    /// <summary>
    /// Use up the remaining coyote time to perform a junmp.
    /// </summary>
    public void UseCoyoteTime() {
      timer = coyoteTime;
    }

    /// <summary>
    /// Set the amount of time the player has to perform a jump after leaving a ledge.
    /// </summary>
    /// <param name="timer">The amount of time the player should have.</param>
    public void SetCoyoteTime(float timer) {
      this.coyoteTime = timer;
    }
    #endregion
  }
}