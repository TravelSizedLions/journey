using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.Characters.Player {

    /**
     *  The main character of the game.
     */
    public class PlayerCharacter : MonoBehaviour {

        #region Player Movement Modes
        //---------------------------------------------------------------------
        // Movement Modes
        //---------------------------------------------------------------------

        public PlayerMovement activeMovementMode;


        public NormalMovement normalMovement;

        public LiveWireMovement liveWireMovement;

        public AimLiveWireMovement aimLiveWireMovement;
        
        public BallisticLiveWireMovement ballisticLiveWireMovement;
        #endregion

        public Rigidbody2D rb;
        public PlayerCollisionSensor sensor;

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

        private void InjectAllModesWithPlayer() {
            foreach(var m in GetComponents<PlayerMovement>()) {
                m.player = this;
            }
        }

        private void ActivateAllModes() {
            foreach(var m in GetComponents<PlayerMovement>()){
                m.Activate();
            }
        }



        private void DeactivateAllModes() {
            // Only the active mode should be enabled on the player.
            foreach(var m in GetComponents<PlayerMovement>()){
                m.Deactivate();
            }
        }


        public void SwitchMovement(PlayerMovementMode mode) {
            DeactivateAllModes();

            switch(mode) {
                case PlayerMovementMode.Normal: 
                    activeMovementMode = normalMovement;
                    break;
                case PlayerMovementMode.LiveWire: 
                    activeMovementMode = liveWireMovement;
                    break;
                case PlayerMovementMode.AimLiveWire:
                    activeMovementMode = aimLiveWireMovement;
                    break;
                case PlayerMovementMode.BallisticLiveWire:
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

