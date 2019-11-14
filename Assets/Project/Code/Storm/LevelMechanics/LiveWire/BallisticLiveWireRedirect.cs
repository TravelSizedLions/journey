using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;

namespace Storm.LevelMechanics.LiveWire {
    public class BallisticLiveWireRedirect : MonoBehaviour
    {

        public Direction forwardMotionDirection;

        public Direction backwardMotionDirection;

        private Vector2 forwardDirection;

        private Vector2 backwardDirection;

        private BoxCollider2D boxCollider;
        private float disableTimer;

        private float delay = 0.125f;

        // Start is called before the first frame update
        void Awake() {
            forwardDirection = Directions2D.toVector(forwardMotionDirection);
            backwardDirection = Directions2D.toVector(backwardMotionDirection);
            boxCollider = GetComponent<BoxCollider2D>();
        }

        public void Update() {
            if (!boxCollider.enabled) {
               disableTimer -= Time.deltaTime;
               if (disableTimer < 0) {
                   boxCollider.enabled = true;
               }
            }
        }

        public void OnTriggerEnter2D(Collider2D col) {
            if (col.CompareTag("Player")) {
                PlayerCharacter player = GameManager.Instance.player;

                // Snap to this node's position;
                player.transform.position = transform.position;

                // Pick which direction to fling the player.
                Vector2 direction = chooseDirection(player.rb.velocity);

                // Switch to Ballistic from directed, if necessary.
                if (player.directedLiveWireMovement.enabled) {
                    player.SwitchBehavior(PlayerBehaviorEnum.BallisticLiveWire);

                    // Fling the player at maximum ballistic speed.
                    player.rb.velocity = player.rb.velocity.normalized*player.aimLiveWireMovement.maxLaunchSpeed;
                }

                if (player.ballisticLiveWireMovement.enabled) {
                    player.transform.position = transform.position;
                    disableTimer = delay;
                    boxCollider.enabled = false;

                    player.ballisticLiveWireMovement.SetDirection(direction);
                } else {
                    player.SwitchBehavior(PlayerBehaviorEnum.BallisticLiveWire);

                    

                    Vector2 initialVelocity = player.aimLiveWireMovement.maxLaunchSpeed*direction;
                    player.ballisticLiveWireMovement.SetInitialVelocity(initialVelocity);
                }
            }
        }


        public Vector2 chooseDirection(Vector2 playerVelocity) {
            if (Directions2D.areOppositeDirections(backwardDirection, playerVelocity)) {
                return forwardDirection;
            } else {
                return backwardDirection;
            }
        }
    }
}
