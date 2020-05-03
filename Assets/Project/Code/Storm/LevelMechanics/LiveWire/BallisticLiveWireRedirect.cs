using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using Storm.Attributes;
using UnityEngine;

namespace Storm.LevelMechanics.Livewire {
  /// <summary>
  /// A Livewire node which flings the player in another direction when the player touches it. The player takes a ballistic trajectory from that point.
  /// </summary>
  /// <seealso cref="LivewireNode" />
  public class BallisticLivewireRedirect : LivewireNode {

    #region Variables
    #region Launch Directions
    [Header("Launch Directions", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The primary (or forward) direction the player will be fired.
    /// </summary>
    [Tooltip("The primary (or forward) direction the player will be fired.")]
    [SerializeField]
    private Direction forwardMotionDirection = Direction.Right;

    /// <summary>
    /// The secondary (or backward) direction the player will be fired.
    /// </summary>
    [Tooltip("The secondary (or backward) direction the player will be fired.")]
    [SerializeField]
    private Direction backwardMotionDirection = Direction.Left;

    /// <summary>
    /// The vector representation of the forward direction.
    /// </summary>
    private Vector2 forwardDirection;
    
    /// <summary>
    /// The vector representation of the backward direction.
    /// </summary>
    private Vector2 backwardDirection;
    #endregion
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();
      forwardDirection = Directions2D.toVector(forwardMotionDirection);
      backwardDirection = Directions2D.toVector(backwardMotionDirection);
    }

    protected override void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        PlayerCharacter player = GameManager.Instance.player;

        // Snap to this node's position;
        player.transform.position = transform.position;

        // Pick which direction to fling the player.
        Vector2 direction = ChooseDirection(player.Rigidbody.velocity);

        // Switch to Ballistic from directed, if necessary.
        if (player.DirectedLiveWireMovement.enabled) {
          player.SwitchBehavior(PlayerBehaviorEnum.BallisticLiveWire);

          // Fling the player at maximum ballistic speed.
          player.Rigidbody.velocity = player.Rigidbody.velocity.normalized * player.AimLiveWireMovement.MaxLaunchSpeed;
        }

        if (player.BallisticLiveWireMovement.enabled) {
          player.transform.position = transform.position;
          disableTimer = delay;
          boxCollider.enabled = false;

          player.BallisticLiveWireMovement.SetDirection(direction);
        } else {
          player.SwitchBehavior(PlayerBehaviorEnum.BallisticLiveWire);

          Vector2 initialVelocity = player.AimLiveWireMovement.MaxLaunchSpeed * direction;
          player.BallisticLiveWireMovement.SetInitialVelocity(initialVelocity);
        }
      }
    }

    /// <summary>
    /// Decide which direction to fling the player.
    /// </summary>
    /// <param name="playerVelocity">The player's velocity vector.</param>
    /// <returns>The direction vector to send the player.</returns>
    private Vector2 ChooseDirection(Vector2 playerVelocity) {
      float backwardDot = Vector2.Dot(backwardDirection, playerVelocity);
      float forwardDot = Vector2.Dot(forwardDirection, playerVelocity);

      // Tests whether the velocity's magnitude is closer to the forward direction or the backward direction.
      float comparison = Mathf.InverseLerp(backwardDot, forwardDot, playerVelocity.magnitude);

      if (comparison < 0.5f) {
        return backwardDirection;
      } else {
        return forwardDirection;
      }
    }

    #endregion
  }
}