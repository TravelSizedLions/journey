using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Attributes;

namespace Storm.Characters.Player {

    /// <summary>
    /// An enum for the different player movement modes. 
    /// Whenever you implement a new movment mode, 
    /// be sure to add a new entry for it in this enum and add it to the switch statement in PlayerMovement.SwitchMovement().
    /// </summary>
    public enum PlayerBehaviorEnum {
        Normal,
        LiveWire,
        AimLiveWire,
        BallisticLiveWire,
    }

    public abstract class PlayerBehavior : MonoBehaviour {

        /// <summary>
        /// A reference back to the player.
        /// </summary>
        [NonSerialized]
        public PlayerCharacter player;

        /// <summary>
        /// The player's Rigidbody.
        /// </summary>
        [NonSerialized]
        public Rigidbody2D rb;

        /// <summary>
        /// The player's BoxCollider.
        /// </summary>
        [NonSerialized]
        public new BoxCollider2D collider;


        /// <summary>
        /// A reference to the player's animator compenent.
        /// 
        /// Since a game object can only have one animator, all player animations for
        /// all player movement behaviors need to reside on that animator.
        /// </summary>
        [NonSerialized]
        public Animator anim;

        /// <summary>
        /// A reference to the player's collision sensor. Used for detecting where collisions are
        /// happening on the player (e.g., top vs. bottom, left vs. right)
        /// </summary>
        [NonSerialized]
        public PlayerCollisionSensor sensor;



        // Start is called before the first frame update
        public virtual void Awake() {
            player = GetComponent<PlayerCharacter>();
            rb = GetComponent<Rigidbody2D>();
            collider = GetComponent<BoxCollider2D>();
            anim = GetComponent<Animator>();
            sensor = GetComponent<PlayerCollisionSensor>();
        }

        // ------------------------------------------------------------------------
        // Mode Activation
        // ------------------------------------------------------------------------

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
    }
}


