using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.Characters.Player {
    public class PlayerCharacter : MonoBehaviour {

        //---------------------------------------------------------------------
        // Movement Modes
        //---------------------------------------------------------------------

        public PlayerMovement activeMovementMode;

        public RealisticMovement realisticMovement;

        public MainframeMovement mainframeMovement;

        public LiveWireMovement liveWireMovement;


        //
        //  
        //

        public Rigidbody2D rb;
        public PlayerCollisionSensor sensor;

        public void Awake() {
            sensor = GetComponent<PlayerCollisionSensor>();
            rb = GetComponent<Rigidbody2D>();
            realisticMovement = GetComponent<RealisticMovement>();
            mainframeMovement = GetComponent<MainframeMovement>();
            liveWireMovement = GetComponent<LiveWireMovement>();
            DeactivateAllModes();
            if (activeMovementMode == null) {
                activeMovementMode = mainframeMovement;
                mainframeMovement.Activate();
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
                case PlayerMovementMode.Realistic: 
                    activeMovementMode = GetComponent<RealisticMovement>();
                    break;
                case PlayerMovementMode.Mainframe: 
                    activeMovementMode = GetComponent<MainframeMovement>();
                    break;
                case PlayerMovementMode.LiveWire: 
                    activeMovementMode = GetComponent<LiveWireMovement>();
                    break;
                default: 
                    activeMovementMode = GetComponent<MainframeMovement>();
                    break;
            }

            activeMovementMode.Activate();
        }
    }
}

