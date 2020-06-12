using Storm.LevelMechanics.Platforms;
using UnityEngine;

namespace Storm.Characters.PlayerOld {
    public class NormalMovementV2 : PlayerBehavior {

        public bool isGrounded;


        public float horizontalSpeed;




        private bool canJump;
        public float singleJumpHeight;

        private Vector2 singleJumpForce;




        public float doubleJumpHeight;

        private Vector2 doubleJumpForce;

        private bool canDoubleJump;


        public void Start() {
            singleJumpForce = new Vector2(0, singleJumpHeight);
            doubleJumpForce = new Vector2(0, doubleJumpHeight);
        }

        public void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                HandleJump();
            }
        }


        public void HandleJump() {
            if (canJump) {
                rigidbody.velocity += singleJumpForce;
                canJump = false;
            } else if (canDoubleJump) {
                rigidbody.velocity += doubleJumpForce;
                canDoubleJump = false;
            }
        }



        public void FixedUpdate() {
            float hAxis = Input.GetAxis("Horizontal");


        }


        //---------------------------------------------------------------------
        // Player Behavior API
        //---------------------------------------------------------------------
        public override void Activate() {

        }

        public override void Deactivate() {

        }

        //---------------------------------------------------------------------
        // Collider Logic
        //---------------------------------------------------------------------

        public void OnCollisionEnter2D(Collision2D other) {
            Vector2 point = other.GetContact(0).point;
            //isGrounded = (Physics2D.OverlapBox() != null);
        }
    }
}