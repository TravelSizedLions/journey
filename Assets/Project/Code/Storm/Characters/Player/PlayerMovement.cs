using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

    public enum PlayerMovementMode {
        Normal,
        LiveWire,
        AimLiveWire,
        BallisticLiveWire,
    }

    public abstract class PlayerMovement : MonoBehaviour {

        /// <summary>
        /// A reference back to the player.
        /// </summary>
        public PlayerCharacter player;

        /// <summary>
        /// The player's Rigidbody.
        /// </summary>
        public Rigidbody2D rb;


        /// <summary>
        /// A reference to the player's animator compenent.
        /// 
        /// Since a game object can only have one animator, all player animations for
        /// all player movement behaviors need to reside on that animator.
        /// </summary>
        public Animator anim;

        /// <summary>
        /// A reference to the player's Box Collider.
        /// </summary>
        public  BoxCollider2D boxCollider;

        /// <summary>
        /// A reference to the player's collision sensor. Used for detecting where collisions are
        /// happening on the player (e.g., top vs. bottom, left vs. right)
        /// </summary>
        protected PlayerCollisionSensor sensor;


        // Start is called before the first frame update
        public virtual void Start() {
            player = GetComponent<PlayerCharacter>();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            sensor = GetComponent<PlayerCollisionSensor>();
        }

        // ------------------------------------------------------------------------
        // Mode Activation
        // ------------------------------------------------------------------------
        public virtual void Deactivate() {
            enabled = false;
        }

        public virtual void Activate() {
            enabled = true;
        }


        // ------------------------------------------------------------------------
        // Player Movement Controls
        // ------------------------------------------------------------------------
    }
}


