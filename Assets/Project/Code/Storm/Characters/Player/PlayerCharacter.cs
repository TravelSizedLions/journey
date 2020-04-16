using System;
using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.Characters.Player {

  /// <summary>
  /// The main character of the game. This class acts as a State Manager for the different Player Behaviors.
  /// </summary>
  /// <seealso cref="PlayerBehavior" /> 
  [RequireComponent(typeof(NormalMovement))]
  [RequireComponent(typeof(DirectedLiveWireMovement))]
  [RequireComponent(typeof(AimLiveWireMovement))]
  [RequireComponent(typeof(BallisticLiveWireMovement))]
  [RequireComponent(typeof(PlayerCollisionSensor))]
  public class PlayerCharacter : MonoBehaviour {

    #region Variables
    #region Player Movement Modes
    //---------------------------------------------------------------------
    // Movement Modes
    //---------------------------------------------------------------------

    /// <summary>
    /// The player behavior that's currently active.
    /// </summary>
    [Tooltip("The player behavior that's currently active. This does not need to be modified in the inspector.")]
    [ReadOnly]
    public PlayerBehavior ActivePlayerBehavior;

    /// <summary>
    /// Jerrod's normal player behavior (running, jumping, etc).
    /// </summary>
    [NonSerialized]
    public NormalMovement NormalMovement;

    /// <summary>
    /// Directed livewire player behavior (shooting from node to node).
    /// </summary>
    [NonSerialized]
    public DirectedLiveWireMovement DirectedLiveWireMovement;

    /// <summary>
    /// Player behavior where Jerrod is locked into launch node,
    /// aiming in a direction to be launched.
    /// </summary>
    [NonSerialized]
    public AimLiveWireMovement AimLiveWireMovement;


    /// <summary>
    /// Player behavior where Jerrod is flying through the air in a
    /// Ballistic arc as a spark of energy. 
    /// This activates after AimLiveWireMovement
    /// </summary>
    [NonSerialized]
    public BallisticLiveWireMovement BallisticLiveWireMovement;

    #endregion

    #region Public Properties
    //---------------------------------------------------------------------
    // Public Properties
    //---------------------------------------------------------------------

    /// <summary>
    /// Jerrod's Rigidbody
    /// </summary>
    [NonSerialized]
    public  Rigidbody2D Rigidbody;

    /// <summary>
    /// Used to sense which direction player collisions are coming from.
    /// </summary>
    [NonSerialized]
    public PlayerCollisionSensor TouchSensor;

    /// <summary>
    /// Used to sense if the player is close to a wall or ceiling.
    /// </summary>
    [NonSerialized]
    public PlayerCollisionSensor ApproachSensor;


    /// <summary> 
    /// Whether or not the player is facing to the right.
    /// </summary>
    [Tooltip("Whether or not the player is facing to the right.")]
    [ReadOnly]
    public bool IsFacingRight;

    #endregion
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      PlayerCollisionSensor[] sensors = GetComponents<PlayerCollisionSensor>();
      TouchSensor = sensors[0];
      ApproachSensor = sensors[1];
      Debug.Log("Approach Sensor: " + ApproachSensor.HorizontalSensitivity);

      Rigidbody = GetComponent<Rigidbody2D>();

      // Get references to PlayerBehaviors
      NormalMovement = GetComponent<NormalMovement>();
      DirectedLiveWireMovement = GetComponent<DirectedLiveWireMovement>();
      AimLiveWireMovement = GetComponent<AimLiveWireMovement>();
      BallisticLiveWireMovement = GetComponent<BallisticLiveWireMovement>();
    }

    private void Start() {
      DisableAllModes();
      if (ActivePlayerBehavior == null) {
        ActivePlayerBehavior = NormalMovement;
        NormalMovement.Activate();
      }
    }

    private void DisableAllModes() {
      foreach (var m in GetComponents<PlayerBehavior>()) {
        m.enabled = false;
      }
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Change which PlayerMovement mode is active on the PlayerCharacter.
    /// </summary>
    /// <param name="mode">An enum for the different PlayerMovement modes.</param>
    public void SwitchBehavior(PlayerBehaviorEnum mode) {
      DeactivateAllModes();

      switch (mode) {
        case PlayerBehaviorEnum.Normal:
          ActivePlayerBehavior = NormalMovement;
          break;
        case PlayerBehaviorEnum.DirectedLiveWire:
          ActivePlayerBehavior = DirectedLiveWireMovement;
          break;
        case PlayerBehaviorEnum.AimLiveWire:
          ActivePlayerBehavior = AimLiveWireMovement;
          break;
        case PlayerBehaviorEnum.BallisticLiveWire:
          ActivePlayerBehavior = BallisticLiveWireMovement;
          break;
        default:
          ActivePlayerBehavior = NormalMovement;
          break;
      }

      ActivePlayerBehavior.Activate();
    }


    /// <summary>
    /// Deactivates every player movement mode that's attached to the PlayerCharacter GameObject.
    /// </summary>
    private void DeactivateAllModes() {
      // Only the active mode should be enabled on the player.
      foreach (var behavior in GetComponents<PlayerBehavior>()) {
        behavior.Deactivate();
      }
    }
    #endregion
  }
}