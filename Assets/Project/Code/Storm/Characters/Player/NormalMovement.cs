using System.Collections;
using System.Collections.Generic;
using Storm.LevelMechanics.Platforms;
using UnityEngine;
using UnityEngine.SceneManagement;


using Storm.Attributes;

namespace Storm.Characters.Player {
    public class NormalMovement : PlayerMovement {

        [Header("Running Parameters", order=0)]
        [Space(5, order=1)]
        #region Player Horizontal Movement Variables

        //---------------------------------------------------------------------
        // Horizontal Movement Variables
        //---------------------------------------------------------------------


        ///<summary>The player's top horizontal speed.</summary>
        [Tooltip("The player's top horizontal speed.")]
        public float maxSpeed;

        /// <summary>The square of maxSpeed.</summary>
        protected float maxSqrVelocity;

        ///<summary>How quickly the player speeds up.</summary>
        [Tooltip("How quickly the player speeds up to the max speed. 0 means no acceleration. 1 means instantaneously.")]
        [Range(0,1)]
        public float acceleration;

        /// <summary> The calculated acceleration value in units/second^2. </summary>
        private float accelerationFactor;

        /// <summary> How quickly the player slows down. </summary>
        [Tooltip("How quickly the player slows down. 0 means no deceleration. 1 means instantaneous stopping.")]
        [Range(0,1)]
        public float deceleration;

        /// <summary> The calculated deceleration force, (1-deceleration, 1). </summary>
        private Vector2 decelerationForce;

        ///<summary>How quickly the player turns around during movement.</summary>
        [Tooltip("How quickly the player turns around during movement.")]
        public float reboundMultiplier;

        /// <summary>Whether or not the player is allowed to move. </summary>
        [Tooltip("Whether or not the player is allowed to move.")]
        [ReadOnly]
        public bool isMovingEnabled;

        /// <summary> Whether or not the player is currently on the ground.</summary>
        [Tooltip("Whether or not the player is currently on the ground.")]
        [ReadOnly]
        public bool isOnGround;

        /// <summary>Whether or not the player is on a moving platform.</summary>
        [Tooltip("Whether or not the player is on a moving platform.")]
        [ReadOnly]
        public bool isOnMovingPlatform;

        [Tooltip("Whether or not the player is facing to the right.")]
        [ReadOnly]
        public bool isFacingRight;

        /// <summary> Whether or not the player is moving. </summary>
        [Tooltip("Whether or not the player is moving.")]
        [ReadOnly]
        public bool isMoving;

        #endregion
        [Space(15, order=2)]



        //---------------------------------------------------------------------------------------//
        // Jump Variables
        //---------------------------------------------------------------------------------------//
        #region Jumping Parameters
        [Header("Jumping Parameters", order=3)]
        [Space(5, order=4)]

        /// <summary> The minimum vertical force to apply to a jump from the ground (tapping jump) </summary>
        [Tooltip("The minimum force to apply to a jump when jumping from the ground (tapping jump)")]
        public float groundShortHop;

        /// <summary> The force variable calculated from groundShortHop. </summary>
        private Vector2 groundShortHopForce;


        /// <summary> The minimum force to apply to a jump when double jumping (tapping jump) </summary>
        [Tooltip("The minimum force to apply to a jump when double jumping (tapping jump)")]
        public float doubleJumpShortHop;

        /// <summary> The force variable calculated from doubleJumpShortHop </summary>
        private Vector2 doubleJumpShortHopForce;



        /// <summary>The additional vertical force added to a jump (when jump button is held). </summary>
        [Tooltip("The additional vertical force added to a jump (when jump button is held).")]
        public float fullHop;

        /// <summary> The force variable calculated from fullHop </summary>
        private Vector2 fullHopForce;



        /// <summary> How long to hold the jump button to perform a full hop (in seconds). </summary>
        [Tooltip("How long the jump button needs to be held to register a full hop.")]
        public float fullHopTime;

        /// <summary>Timer used to time the check for a full hop.</summary>
        private float fullHopTimer;

        /// <summary>
        /// Used to prevent full hop from being applied to a jump more than once.
        /// Calculated to be shortly after fullHopTime
        /// </summary>
        private float jumpTimeMax;

        /// <summary>
        /// A timer used to prevent multiple repeated jump inputs.
        /// </summary>
        private float jumpTimer;

        /// <summary>
        /// Whether or not the player is allowed to jump.
        /// </summary>
        [Tooltip("Whether or not the player is allowed to jump.")]
        [ReadOnly]
        public bool isJumpingEnabled;

        /// <summary>
        /// Whether or not the player has performed their first jump.
        /// </summary>
        [Tooltip("Whether or not the player has performed their first jump.")]
        [ReadOnly]
        public bool hasJumped;

        /// <summary>
        /// Whether or not the player is in a position to double jump.
        /// </summary>
        [Tooltip("Whether or not the player is in a position to double jump.")]
        [ReadOnly]
        public bool canDoubleJump;

        /// <summary>
        /// Whether or not the player has performed their second jump.
        /// </summary>
        [Tooltip("Whether or not the player has performed their second jump.")]
        [ReadOnly]
        public bool hasDoubleJumped;

        /// <summary>
        /// Whether the jump input has been pressed (edge-triggered).
        /// </summary>
        [Tooltip("Whether the jump input has been pressed (edge-triggered).")]
        [ReadOnly]
        public bool jumpInputPressed;

        /// <summary>
        /// Whether the jump input has been pressed (level-triggered).
        /// </summary>
        [Tooltip("Whether the jump input has been pressed (level-triggered).")]
        [ReadOnly]
        public bool jumpInputHeld;

        /// <summary>
        /// Whether the jump input has been released (edge triggered).
        /// </summary>
        [Tooltip("Whether the jump input has been released (edge triggered).")]
        [ReadOnly]
        public bool jumpInputReleased;

        #endregion
        [Space(15, order=5)]


        #region Wall Interactions
        [Header("Wall Action Parameters", order=6)]
        [Space(5, order=7)]

        /// <summary>
        /// How quickly the player slides down the wall. 0 - no friction, 1 - player sticks to wall
        /// </summary>
        [Tooltip("How quickly the player slides down the wall. 0 - no friction, 1 - player sticks to wall")]
        [Range(0,1)]
        public float wallFriction;

        /// <summary>
        /// Vertical force applied to a wall jump.
        /// </summary>
        [Tooltip("Vertical force applied to a wall jump.")]
        public float wallJump;

        /// <summary>
        /// Force vector calculated from wallJump
        /// </summary>
        private Vector2 wallJumpForce;

        /// <summary>
        /// How easy it is for the player to get back to the wall after a
        /// wall jump. Higher is easier.
        /// </summary>
        [Tooltip("How easy it is for the player to get back to the wall after a wall jump. Higher is easier.")]
        public float wallJumpMuting;

        /// <summary>
        /// Whether or not the player is touching a left-hand wall.
        /// </summary>
        [Tooltip("Whether or not the player is touching a left-hand wall.")]
        [ReadOnly]
        public bool isOnLeftWall;

        /// <summary>
        /// Whether or not the player is touching a right-hand wall.
        /// </summary>
        [Tooltip("Whether or not the player is touching a right-hand wall.")]
        [ReadOnly]
        public bool isOnRightWall;
        
        /// <summary>
        /// Whether or not the player is in the middle of a wall jump.
        /// </summary>
        [Tooltip("Whether or not the player is in the middle of a wall jump.")]
        [ReadOnly]
        public bool isWallJumping;

        /// <summary>
        /// Whether or not the player is jumping from wall to wall.
        /// </summary>
        [Tooltip("Whether or not the player is jumping from wall to wall.")]
        [ReadOnly]
        public bool isInWallJumpCombo;

        #endregion



        #region PlayerMovement Method Overrides

        public override void Activate() {
            base.Activate();
            resetJumpLogic();
        }

        public override void Deactivate() {
            base.Deactivate();
        }

        #endregion


        #region Unity Methods
        //---------------------------------------------------------------------
        // Unity Methods
        //---------------------------------------------------------------------

        public override void Start() {
            base.Start();

            isFacingRight = GameManager.Instance.transitions.GetCurrentSpawnFacing();
            anim.SetBool("IsFacingRight", isFacingRight);

            rb.freezeRotation = true;

            transform.position = GameManager.Instance.transitions.GetCurrentSpawnPosition();

            wallFriction = 1-wallFriction;

            accelerationFactor = maxSpeed*acceleration;
            decelerationForce = new Vector2(1-deceleration, 1);
            maxSqrVelocity = maxSpeed*maxSpeed;
    
            // Used to restrain full hop from being applied more than once.
            jumpTimeMax = fullHopTime + 2*Time.fixedDeltaTime;

            wallJumpForce = new Vector2(wallJump, 0);
            groundShortHopForce = new Vector2(0, groundShortHop);
            fullHopForce = new Vector2(0, fullHop);
            doubleJumpShortHopForce = new Vector2(0, doubleJumpShortHop);
        }


        protected void Update() {
            collectInput();
        }

        protected void FixedUpdate() {
            sensor.sense();
            updateAnimator();
            moveCalculations();
            jumpCalculations();
        }

        #endregion

        protected void collectInput() {
            jumpInputPressed = Input.GetKeyDown(KeyCode.Space) || jumpInputPressed;
            jumpInputHeld = Input.GetKey(KeyCode.Space);
            jumpInputReleased = Input.GetKeyUp(KeyCode.Space) || jumpInputReleased;
        }

        /*
         * Determine the player's situation for animation purposes.
         */
        protected void updateAnimator() {

            // Check movement
            float motion = Mathf.Abs(rb.velocity.x);
            isMoving = motion > 0.3f;
            anim.SetBool("IsMoving", isMoving);


            // Check whether facing left or right
            if (!anim.GetBool("IsFacingRight") && rb.velocity.x > 0.1) {
                anim.SetBool("IsFacingRight", true);
            } else if (rb.velocity.x < -0.1) {
                anim.SetBool("IsFacingRight", false);
            }

            // Update player facing information for camera
            //Debug.Log(rb.velocity.x);
            if (isOnGround) {
                if (rb.velocity.x < -0.1) {
                    isFacingRight = false;
                } else if (rb.velocity.x > 0.1) {
                    isFacingRight = true;
                }
                // zero case: leave boolean as is
            }
            //Debug.Log(isFacingRight);
            
            // Check if the character is touching a wall.
            isOnGround = sensor.isTouchingFloor();
            isOnLeftWall = sensor.isTouchingLeftWall() && !sensor.isTouchingCeiling();
            isOnRightWall = sensor.isTouchingRightWall() && !sensor.isTouchingCeiling();
            anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
            anim.SetBool("IsTouchingRightWall", isOnRightWall);

            if (isOnRightWall || isOnLeftWall) {
                anim.SetBool("IsFalling", false);
                anim.SetBool("IsJumping", false);
                anim.SetBool("IsDoubleJumping", false);
            } 
            
            if (isOnGround) {

                // The character is on the ground.
                anim.SetBool("IsJumping", false);
                anim.SetBool("IsDoubleJumping", false);
                anim.SetBool("IsFalling", false);
                anim.SetBool("IsOnGround", true);

                if (isOnLeftWall || isOnRightWall) {
                    //Debug.Log("Is on a wall");
                    if (rb.velocity.y < 0) {
                        anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
                        anim.SetBool("IsTouchingRightWall", isOnRightWall);
                    }
                }

            } else { 
                
                // The character is in the air.
                
                // Check Whether the character is rising or falling.  
                //Debug.Log(rb.velocity.y);
                if (isInWallJumpCombo && (isOnLeftWall || isOnRightWall)) {
                    anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
                    anim.SetBool("IsTouchingRightWall", isOnRightWall);
                } else if (isOnLeftWall || isOnRightWall) {
                    //Debug.Log("Is on a wall");
                    if (rb.velocity.y > -0.75f) {
                        if (hasDoubleJumped) {
                            anim.SetBool("IsDoubleJumping", true);
                        } else {
                            anim.SetBool("IsJumping", true);
                        }
                        anim.SetBool("IsFalling", false);
                        anim.SetBool("IsOnGround", false);
                    } else {
                        anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
                        anim.SetBool("IsTouchingRightWall", isOnRightWall);
                    }
                } else {
                    if (rb.velocity.y > 0) {
                        if (hasDoubleJumped) {
                            anim.SetBool("IsDoubleJumping", true);
                        } else {
                            anim.SetBool("IsJumping", true);
                        }
                        anim.SetBool("IsFalling", false);
                        anim.SetBool("IsOnGround", false);
                    } else if (rb.velocity.y < 0 ) {
                        anim.SetBool("IsJumping", false);
                        anim.SetBool("IsDoubleJumping", false);
                        anim.SetBool("IsFalling", true);
                        anim.SetBool("IsOnGround", false);  
                    } 
                }
                    
            }

            if (isOnGround && (isOnLeftWall || isOnRightWall) && rb.velocity.y < 0) {
                isOnGround = false;
                anim.SetBool("IsOnGround", false);
                anim.SetBool("IsTouchingLeftWall", isOnLeftWall);
                anim.SetBool("IsTouchingRightWall", isOnRightWall);
            }


        }

        protected void moveCalculations() {
            // Move the player.
            if (!isMovingEnabled) {
                rb.velocity *= decelerationForce; 
                return;
            }

            float input = Input.GetAxis("Horizontal");
            if (!isOnMovingPlatform && input != 0) {
                transform.SetParent(null);
            }

            float inputDirection = Mathf.Sign(input);

            // decelerate.
            if (Mathf.Abs(input) != 1 && !isWallJumping) { 
                rb.velocity *= decelerationForce; 
                return;

            }

            // Get player direction.
            float motionDirection = Mathf.Sign(rb.velocity.x);


            // If the player is turning around, apply more force
            float adjustedInput = inputDirection == motionDirection ? input : input*reboundMultiplier;
            if (isWallJumping) {
                adjustedInput *= wallJumpMuting;
            }

            // calculate new speed of player, accounting for maximum speed.
            if (inputDirection == -1 && isOnLeftWall) {
                rb.velocity = Vector2.up*rb.velocity;
            } else if (inputDirection == 1 && isOnRightWall) {
                rb.velocity = Vector2.up*rb.velocity;
            } else {
                float horizSpeed = Mathf.Clamp(rb.velocity.x+adjustedInput*accelerationFactor, -maxSpeed, maxSpeed);
                rb.velocity = new Vector2(horizSpeed, rb.velocity.y);
            }
            

        }

        protected void jumpCalculations() {
            if (!isJumpingEnabled) {
                jumpInputPressed = false;
                jumpInputHeld = false;
                jumpInputReleased = false;
                return;
            }

            if(isOnGround) {
                isInWallJumpCombo = false;
                resetJumpLogic();
            } else {
                bool isOnWall = false;                
                Vector2 moveForce = Vector2.zero;
                float movement = rb.velocity.x;

                // Keeps the character from moving into the wall continually
                // (manifests itself as sticking to the wall).
                if (isOnLeftWall) {
                    isOnWall = true;
                    moveForce = new Vector2(movement > 0 ? movement : 0, 0);
                } else if (isOnRightWall) {
                    // Debug.Log("Is on wall");
                    isOnWall = true;
                    moveForce = new Vector2(movement < 0 ? movement : 0, 0);
                }

                if (isOnWall && rb.velocity.y < 0) {
                    resetJumpLogic();
                    rb.velocity = rb.velocity*Vector2.up*wallFriction + moveForce;
                }
            }

            if (jumpInputPressed) {
                jumpInputPressed = false;

                // is on the ground/on a wall
                if (!hasJumped) { 

                    hasJumped = true;
                    jumpTimer = 0;
                    fullHopTimer = 0;
                    rb.velocity = rb.velocity*Vector2.right+groundShortHopForce;

                    if (!isOnGround && isOnRightWall) {

                        rb.velocity -= wallJumpForce;
                        isOnRightWall = false;
                        isWallJumping = true;
                        isInWallJumpCombo = true;
                    } else if (!isOnGround && isOnLeftWall) {

                        rb.velocity += wallJumpForce;
                        isOnLeftWall = false;
                        isWallJumping = true;
                        isInWallJumpCombo = true;
                    } 

                // hasn't double jumped yet.
                } else if (canDoubleJump) {
                    jumpTimer = 0;
                    fullHopTimer = 0;

                    if (isOnLeftWall) {

                        hasJumped = true;
                        canDoubleJump = false;
                        rb.velocity = rb.velocity*Vector2.right+groundShortHopForce;
                        rb.velocity += wallJumpForce;   
                        isOnLeftWall = false;
                        isWallJumping = true;
                        isInWallJumpCombo = true;

                    } else if (isOnRightWall) {

                        hasJumped = true;
                        canDoubleJump = false;
                        rb.velocity = rb.velocity*Vector2.right+groundShortHopForce;
                        rb.velocity -= wallJumpForce;
                        isOnRightWall = false;
                        isWallJumping = true;
                        isInWallJumpCombo = true;

                    } else  {

                        rb.velocity = rb.velocity*Vector2.right+doubleJumpShortHopForce;
                        hasJumped = true;
                        canDoubleJump = false;
                        hasDoubleJumped = true;
                        isWallJumping = false;
                    } 

                }

            }

            if (jumpInputHeld && 
                jumpTimer < jumpTimeMax &&
                fullHopTimer > fullHopTime) {
                    
                // Perform a full hop instead of a full hop.
                fullHopTimer = 0;
                rb.velocity = rb.velocity*Vector2.right + fullHopForce;
            }

            fullHopTimer += Time.fixedDeltaTime;
            jumpTimer += Time.fixedDeltaTime;
            if (jumpInputReleased) {

                if (isOnLeftWall || isOnRightWall) {
                    //Debug.Log("if (isOnLeftWall || isOnRightWall) {");
                    resetJumpLogic();
                } else if (!hasDoubleJumped) {
                    canDoubleJump = true;
                }
            }

        }

        public void EnableMoving() {
            isMovingEnabled = true;
        }

        public void DisableMoving() {
            isMovingEnabled = false;
        }


        public void DisableJump() {
            isJumpingEnabled = false;
        }


        public void EnableJump() {
            isJumpingEnabled = true;
        }


        public void resetJumpLogic() {
            // Allow the character 
            jumpTimer = 0;
            fullHopTimer = 0;
            hasJumped = false;
            hasDoubleJumped = false;
            canDoubleJump = false;
            isWallJumping = false;
        }


        /** Keeps a player's movement tethered to a moving platform in the air. */
        public void EnablePlatformMomentum() {
            isOnMovingPlatform = true;
        }

        /** Removes player association with a moving platform. */
        public void DisablePlatformMomentum() {
            isOnMovingPlatform = false;
        }

        public void OnCollisionEnter2D(Collision2D collision) {
            
            // Catches the case where the player lands on solid ground from a 
            // moving platform without directional input.
            if (collision.collider.GetComponent<MovingPlatform>() == null) {
                DisablePlatformMomentum();
                transform.SetParent(null);
            }
        }
    }
}
