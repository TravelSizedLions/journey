using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Storm.Attributes;

namespace Storm.Characters.Player {

    /// <summary>
    /// The main Character of the game.
    /// </summary>
    [RequireComponent(typeof(NormalMovement))]
    [RequireComponent(typeof(LiveWireMovement))]
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
        public PlayerMovement activeMovementMode;

        /// <summary>
        /// Jerrod's normal player behavior (running, jumping, etc).
        /// </summary>
        [NonSerialized]
        public NormalMovement normalMovement;

        /// <summary>
        /// Directed livewire player behavior (shooting from node to node).
        /// </summary>
        [NonSerialized]
        public LiveWireMovement liveWireMovement;

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
        public PlayerCollisionSensor sensor;

        #endregion


        public void Awake() {
            sensor = GetComponent<PlayerCollisionSensor>();
            rb = GetComponent<Rigidbody2D>();

            normalMovement = GetComponent<NormalMovement>();
            liveWireMovement = GetComponent<LiveWireMovement>();
            aimLiveWireMovement = GetComponent<AimLiveWireMovement>();
            ballisticLiveWireMovement = GetComponent<BallisticLiveWireMovement>();

            InjectAllModesWithPlayer();
            DeactivateAllModes();
            if (activeMovementMode == null) {
                activeMovementMode = normalMovement;
                normalMovement.Activate();
            }
        }


        /// <summary>
        /// Adds a reference to PlayerCharacter to all attached PlayerMovement scripts.
        /// </summary>
        private void InjectAllModesWithPlayer() {
            foreach(var m in GetComponents<PlayerMovement>()) {
                m.player = this;
            }
        }


        /// <summary>
        /// Deactivates every player movement mode that's attached to the PlayerCharacter GameObject.
        /// </summary>
        private void DeactivateAllModes() {
            // Only the active mode should be enabled on the player.
            foreach(var m in GetComponents<PlayerMovement>()){
                m.Deactivate();
            }
        }


        /// <summary>
        /// Change which PlayerMovement mode is active on the PlayerCharacter.
        /// </summary>
        /// <param name="mode">An enum for the different PlayerMovement modes.</param>
        public void SwitchBehavior(PlayerMovementEnum mode) {
            DeactivateAllModes();

            switch(mode) {
                case PlayerMovementEnum.Normal: 
                    activeMovementMode = normalMovement;
                    break;
                case PlayerMovementEnum.LiveWire: 
                    activeMovementMode = liveWireMovement;
                    break;
                case PlayerMovementEnum.AimLiveWire:
                    activeMovementMode = aimLiveWireMovement;
                    break;
                case PlayerMovementEnum.BallisticLiveWire:
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

