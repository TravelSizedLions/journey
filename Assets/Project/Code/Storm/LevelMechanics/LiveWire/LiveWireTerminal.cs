using Storm.Characters.PlayerOld;
using UnityEngine;


namespace Storm.LevelMechanics.Livewire {
  
  /// <summary>
  /// Transforms the character into a spark of energy and sends him in a specific direction. The player is not affected by gravity in this state.
  /// </summary>
  /// <seealso cref="LivewireNode" />
  public class LivewireTerminal : LivewireNode {

    #region Variables

    /// <summary>
    /// The direction the player will be sent.
    /// </summary>
    [SerializeField]
    private Direction motionDirection = Direction.Right;

    /// <summary>
    /// The direction the player pops out of the node when exiting livewire behavior.
    /// </summary>
    [SerializeField]
    private Direction exitDirection = Direction.Left;

    /// <summary>
    /// The vector representation of the motion direction.
    /// </summary>
    private Vector2 direction;

    #endregion

    #region Constants

    /// <summary>
    /// The start angle at which the player could exit livewire mode from.
    /// </summary>
    private const float START_ANGLE = 135;

    /// <summary>
    /// The end angle at which the player could exit livewire mode from.
    /// </summary>
    private const float END_ANGLE = 225;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();
      direction = Directions2D.toVector(motionDirection);
    }

    protected override void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        PlayerCharacterOld player = FindObjectOfType<PlayerCharacterOld>();
        player.transform.position = transform.position;
        disableTimer = delay;
        boxCollider.enabled = false;

        // Transition to or from Livewire mode.
        // Debug.Log("Direction: " + direction + ", velocity: " + player.Rigidbody.velocity);
        

        // Check the angle of player approach to determine if the player is entering or exiting livewire mode.
        float angleBetween = Directions2D.AngleBetween(direction, player.Rigidbody.velocity);

        if (player.DirectedLiveWireMovement.enabled ||
          (player.BallisticLiveWireMovement.enabled && (angleBetween > START_ANGLE && angleBetween < END_ANGLE))) {


          player.SwitchBehavior(PlayerBehaviorEnum.Normal);

          player.NormalMovement.ExitLiveWire(exitDirection);

        } else {
          Debug.Log("To LiveWire");
          player.SwitchBehavior(PlayerBehaviorEnum.DirectedLiveWire);

          player.DirectedLiveWireMovement.SetDirection(direction);
        }
      }
    }

    #endregion

  }
}