using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

    public enum PlayerMovementMode {
        Realistic,
        Mainframe,
        LiveWire,
    }

    public abstract class PlayerMovement : MonoBehaviour {

        // ------------------------------------------------------------------------
        // Component Variables
        // ------------------------------------------------------------------------
        public Rigidbody2D rb;

        public Animator anim;

        public  BoxCollider2D boxCollider;

        protected PlayerCollisionSensor sensor;

        // ------------------------------------------------------------------------
        // Player Movement Variables 
        // ------------------------------------------------------------------------

        // The acceleration used in speeding up
        public float acceleration;

        // The player's max speed.
        public float maxVelocity;

        protected float maxSqrVelocity;

        // The deceleration applied to slowing down.
        public Vector2 deceleration;

        // Controls how fast the player turns around during movement.
        public float rebound;

        // ------------------------------------------------------------------------
        // Jumping Variables
        // ------------------------------------------------------------------------
        
        // The vertical force to apply to a jump.
        public float jump;
        
        protected Vector2 jumpForce;

        // ------------------------------------------------------------------------
        // Raycasting Variables
        // ------------------------------------------------------------------------

        // Measures the center of the player to the ground.
        protected float distanceToGround;

        // Determines how sensitive ground raycasting is.
        public float raycastBuffer;

        // Determines which layers to raycast on.
        public LayerMask groundLayerMask;

        // ------------------------------------------------------------------------
        // Player Orientation Information
        // ------------------------------------------------------------------------
        public bool isFacingRight;

        public bool isOnGround;

        // ------------------------------------------------------------------------
        // Mechanic Controls
        // ------------------------------------------------------------------------
        public bool isJumpingEnabled;

        public bool isMovingEnabled;

        public bool isPlatformMomentumEnabled;

        #region Sensing Variables
        //---------------------------------------------------------------------------------------//
        //  Sensing Variables
        //---------------------------------------------------------------------------------------//
        private Vector3 bottomLeft;

        private Vector3 bottomRight;

        private Vector3 topLeft;

        private Vector3 topRight;
        #endregion



        // Start is called before the first frame update
        public virtual void Start() {
            transform.position = GameManager.Instance.transitions.getSpawnPosition();
            isFacingRight = GameManager.Instance.transitions.getSpawningRight();
            anim = GetComponent<Animator>();
            anim.SetBool("IsFacingRight", isFacingRight);

            jumpForce = new Vector2(0, jump);
            maxSqrVelocity = maxVelocity*maxVelocity;
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;

            sensor = GetComponent<PlayerCollisionSensor>();
            boxCollider = GetComponent<BoxCollider2D>();
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

        public void EnableJump() {
            isJumpingEnabled = true;
        }

        public void DisableJump() {
            isJumpingEnabled = false;
        }

        public void EnableMoving() {
            isMovingEnabled = true;
        }

        public void DisableMoving() {
            isMovingEnabled = false;
        }


        /** Keeps a player's movement tethered to a moving platform in the air. */
        public void EnablePlatformMomentum() {
            isPlatformMomentumEnabled = true;
        }

        /** Removes player association with a moving platform. */
        public void DisablePlatformMomentum() {
            isPlatformMomentumEnabled = false;
        }
    }
}


