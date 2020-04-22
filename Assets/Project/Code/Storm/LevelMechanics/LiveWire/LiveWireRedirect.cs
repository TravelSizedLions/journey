using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.LevelMechanics.LiveWire {

  /// <summary>
  /// A Livewire node which redirects the player from one direction to another. The player continues travelling in a straight line after being redirected.
  /// </summary>
  public class LivewireRedirect : LivewireNode {

    #region Launch Direction Variables
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

        if (!(player.DirectedLiveWireMovement.enabled || player.BallisticLiveWireMovement.enabled)) {
          return;
        }

        if (player.BallisticLiveWireMovement.enabled) {
          player.SwitchBehavior(PlayerBehaviorEnum.DirectedLiveWire);
        }

        player.transform.position = transform.position;
        disableTimer = delay;
        boxCollider.enabled = false;

        if (Directions2D.AreOppositeDirections(backwardDirection, player.Rigidbody.velocity)) {
          player.DirectedLiveWireMovement.SetDirection(forwardDirection);
        } else if (Directions2D.AreOppositeDirections(forwardDirection, player.Rigidbody.velocity)) {
          player.DirectedLiveWireMovement.SetDirection(backwardDirection);
        } else {
          player.DirectedLiveWireMovement.SetDirection(forwardDirection);
        }
      }
    }

    #endregion
  }

}