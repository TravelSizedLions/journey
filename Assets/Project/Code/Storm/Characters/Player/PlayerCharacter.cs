using System;
using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.Characters.Player {

  /// <summary>
  /// The main character of the game.
  /// </summary>
  [RequireComponent(typeof(NormalMovement))]
  [RequireComponent(typeof(DirectedLiveWireMovement))]
  [RequireComponent(typeof(AimLiveWireMovement))]
  [RequireComponent(typeof(BallisticLiveWireMovement))]
  [RequireComponent(typeof(PlayerCollisionSensor))]
  public class PlayerCharacter : MonoBehaviour {

    #region Player Movement Modes
    //---------------------------------------------------------------------
    // Movement Modes
    //---------------------------------------------------------------------

    /// <summary>
    /// The player behavior that's currently active.
    /// </summary>
    [Tooltip("The player behavior that's currently active. This does not need to be modified in the inspector.")]
    [ReadOnly]
    public PlayerBehavior activeMovementMode;

    /// <summary>
    /// Jerrod's normal player behavior (running, jumping, etc).
    /// </summary>
    [NonSerialized]
    public NormalMovement normalMovement;

    /// <summary>
    /// Directed livewire player behavior (shooting from node to node).
    /// </summary>
    [NonSerialized]
    public DirectedLiveWireMovement directedLiveWireMovement;

    /// <summary>
    /// Player behavior where Jerrod is locked into launch node,
    /// aiming in a direction to be launched.
    /// </summary>
    [NonSerialized]
    public AimLiveWireMovement aimLiveWireMovement;


    /// <summary>
    /// Player behavior where Jerrod is flying through the air in a
    /// Ballistic arc as a spark of energy. 
    /// This activates after AimLiveWireMovement
    /// </summary>
    [NonSerialized]
    public BallisticLiveWireMovement ballisticLiveWireMovement;

    #endregion

    #region Public Properties
    //---------------------------------------------------------------------
    // Public Properties
    //---------------------------------------------------------------------

    /// <summary>
    /// Jerrod's Rigidbody
    /// </summary>
    [NonSerialized]
    public Rigidbody2D rb;

    /// <summary>
    /// A class used to sense which direction player collisions are coming from.
    /// </summary>
    [NonSerialized]
    public PlayerCollisionSensor touchSensor;

    public PlayerCollisionSensor approachSensor;


    /// <summary> Whether or not the player is facing to the right.</summary>
    [Tooltip("Whether or not the player is facing to the right.")]
    [ReadOnly]
    public bool isFacingRight;

    #endregion


    public void Awake() {
      PlayerCollisionSensor[] sensors = GetComponents<PlayerCollisionSensor>();
      touchSensor = sensors[0];
      approachSensor = sensors[1];
      Debug.Log("Approach Sensor: " + approachSensor.horizontalSensitivity);

      rb = GetComponent<Rigidbody2D>();

      // Get references to PlayerBehaviors
      normalMovement = GetComponent<NormalMovement>();
      directedLiveWireMovement = GetComponent<DirectedLiveWireMovement>();
      aimLiveWireMovement = GetComponent<AimLiveWireMovement>();
      ballisticLiveWireMovement = GetComponent<BallisticLiveWireMovement>();
    }

    public void Start() {
      DisableAllModes();
      if (activeMovementMode == null) {
        activeMovementMode = normalMovement;
        normalMovement.Activate();
      }
    }


    /// <summary>
    /// Deactivates every player movement mode that's attached to the PlayerCharacter GameObject.
    /// </summary>
    private void DeactivateAllModes() {
      // Only the active mode should be enabled on the player.
      foreach (var m in GetComponents<PlayerBehavior>()) {
        m.Deactivate();
      }
    }


    private void DisableAllModes() {
      foreach (var m in GetComponents<PlayerBehavior>()) {
        m.enabled = false;
      }
    }


    /// <summary>
    /// Change which PlayerMovement mode is active on the PlayerCharacter.
    /// </summary>
    /// <param name="mode">An enum for the different PlayerMovement modes.</param>
    public void SwitchBehavior(PlayerBehaviorEnum mode) {
      DeactivateAllModes();

      switch (mode) {
        case PlayerBehaviorEnum.Normal:
          activeMovementMode = normalMovement;
          break;
        case PlayerBehaviorEnum.DirectedLiveWire:
          activeMovementMode = directedLiveWireMovement;
          break;
        case PlayerBehaviorEnum.AimLiveWire:
          activeMovementMode = aimLiveWireMovement;
          break;
        case PlayerBehaviorEnum.BallisticLiveWire:
          activeMovementMode = ballisticLiveWireMovement;
          break;
        default:
          activeMovementMode = normalMovement;
          break;
      }

      activeMovementMode.Activate();
    }
  }
}