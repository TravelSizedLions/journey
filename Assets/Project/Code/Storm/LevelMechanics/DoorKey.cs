using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.ResetSystem;
using Storm.Attributes;

namespace Storm.LevelMechanics {

    /// <summary>
    /// A key for a locked door. Whenever the key is 
    /// collected, it will signal to the door that it's 
    /// been collected so the door can open.
    /// </summary>
    public class DoorKey : Resetting {

        /// <summary>
        /// The door that this key unlocks.
        /// </summary>
        [ReadOnly]
        public LockedDoor door;


        /// <summary>
        /// A reference to the sprite
        /// </summary>
        [ReadOnly]
        public SpriteRenderer sprite;


        /// <summary>
        /// Whether or not this key has been collected by the player.
        /// </summary>
        [ReadOnly]
        public bool isCollected;

        public void Awake() {
            this.sprite = GetComponent<SpriteRenderer>();
        }


        /// <summary>
        /// Collect the key when the player collides with it.
        /// </summary>
        public void OnTriggerEnter2D(Collider2D col) {
            if (col.CompareTag("Player")) {
                Debug.Log("Collected!");
                this.sprite.enabled = false;
                this.isCollected = true;

                this.door.OnKeyCollected();
            }
        }

        /// <summary>
        /// Make the key re-appear if the player dies before
        /// opening the door.
        /// </summary>
        public override void Reset() {
            if (!this.door.IsOpen()) {
                this.isCollected = false;
                this.sprite.enabled = true;
            }
        }
    }
}