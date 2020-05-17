using System;
using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using UnityEngine;

namespace Storm.Characters.PlayerOld {

  /// <summary>
  /// IMPORTANT:     
  /// An enum for the different player movement modes. 
  /// Whenever you implement a new movment mode, 
  /// be sure to add a new entry for it in this enum and add it to the switch statement in PlayerMovement.SwitchMovement().
  /// </summary>
  public enum PlayerBehaviorEnum {
    Normal,
    DirectedLiveWire,
    AimLiveWire,
    BallisticLiveWire,
  }

  /// <summary>
  /// The base class for all player based behavior. If an interaction in the game requires direct player input or changes the way the
  /// player moves, such as aiming, running, climbing, or flying, then inherit from this class and follow these steps:
  /// 1. Implement the desired behavior in the deriving class
  /// 2. Implement the activate/deactivate methods to deal with setting/resetting state for your behavior
  /// 3. Add your Player Behavior script to the Player Prefab
  /// 4. Add a public field for your behavior within the PlayerCharacter class
  /// 5. Add a [RequiredComponent] attribute to the top of the PlayerCharacter class
  /// 6. In PlayerCharacter's Awake() method, get a reference to your new Player Behavior Component
  /// 7. Finally, add an entry in the PlayerBehaviorEnum in PlayerBehavior.cs, and add a case for it in PlayerCharacter.SwitchBehavior().
  /// </summary>
  [RequireComponent(typeof(PlayerCharacterOld))]
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(BoxCollider2D))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(PlayerCollisionSensor))]
  public abstract class PlayerBehavior : MonoBehaviour {

    #region Components
    /// <summary>
    /// A reference back to the player.
    /// </summary>
    [NonSerialized]
    protected PlayerCharacterOld player;

    /// <summary>
    /// The player's Rigidbody.
    /// </summary>
    [NonSerialized]
    public new Rigidbody2D rigidbody;

    /// <summary>
    /// The player's BoxCollider.
    /// </summary>/
    [NonSerialized]
    protected new BoxCollider2D collider;


    /// <summary>
    /// A reference to the player's animator component.
    /// 
    /// Since a game object can only have one animator, all player animations for
    /// all player movement behaviors need to reside on that animator.
    /// </summary>
    [NonSerialized]
    protected Animator anim;

    /// <summary>
    /// A reference to the player's first collision sensor. Used for detecting where collisions are
    /// happening on the player (e.g., top vs. bottom, left vs. right)
    /// </summary>
    [NonSerialized]
    protected PlayerCollisionSensor touchSensor;

    /// <summary>
    /// A reference to the player's second collision sensor. Used for detecting when a player is approaching a 
    /// wall/boundary.
    /// </summary>
    [NonSerialized]
    protected PlayerCollisionSensor approachSensor;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------//
    // Unity API
    //-------------------------------------------------------------------//
    protected virtual void Awake() {
      player = GetComponent<PlayerCharacterOld>();
      rigidbody = GetComponent<Rigidbody2D>();
      collider = GetComponent<BoxCollider2D>();
      anim = GetComponent<Animator>();

      PlayerCollisionSensor[] sensors = GetComponents<PlayerCollisionSensor>();

      touchSensor = sensors[0];
      approachSensor = sensors[1];

    }
    #endregion

    #region Player Behavior API
    // ------------------------------------------------------------------//
    // Mode Activation & Deactivation
    // ------------------------------------------------------------------//

    /// <summary>
    /// Fires whenever a player switches to a particular player behavior. Use this to perform re-activation logic.
    /// </summary>
    public virtual void Activate() {
      enabled = true;
    }


    /// <summary>
    /// Fires whenever a player switches away from a particular player behavior. Use this to perform deactivation logic (resetting parameters, etc).
    /// </summary>
    public virtual void Deactivate() {
      enabled = false;
    }
    #endregion
  }
}