using System.Collections;
using System.Collections.Generic;
using Storm.LevelMechanics.Platforms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.Characters.Player {
    public class MainframeMovement : PlayerMovement {

        public bool isMoving;

        #region Wall Slide Variables
        //---------------------------------------------------------------------------------------//
        //  Wall Slide Variables
        //---------------------------------------------------------------------------------------//
        private float distanceToWall;

        public bool isOnLeftWall;
        
        public bool isOnRightWall;
        
        public float wallFriction;

        #endregion



        #region Wall Jump Variables
        //---------------------------------------------------------------------------------------//
        //  Wall Jump Variables
        //---------------------------------------------------------------------------------------//

        public bool isWallJumping;

        public float wallJumpAccelerationModifier;

        public float wallJump;

        private Vector2 wallJumpForce;

        public bool isInWallJumpCombo;

        #endregion



        #region Jump Variables
        //---------------------------------------------------------------------------------------//
        // Jump Variables
        //---------------------------------------------------------------------------------------//
        public bool jumpInputPressed;

        public bool jumpInputHeld;

        public bool jumpInputReleased;

        private float jumpRecheckTimer;
        public float recheckJump;
        private float jumpTimer;

        public float jumpInputDelay;

        public float jumpTimeMax;

        public bool hasJumped;

        public float shortHop;

        private Vector2 shortHopForce;
        public float  fullHop;

        private Vector2 fullHopForce;

        #endregion



        #region Double Jump Variables
        //---------------------------------------------------------------------------------------//
        //  Double Jump Variables
        //---------------------------------------------------------------------------------------//

        public bool canDoubleJump;

        public bool hasDoubleJumped;

        public float doubleJumpShortHop;

        private Vector2 doubleJumpShortHopForce;

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

            wallJumpForce = new Vector2(wallJump, 0);
            shortHopForce = new Vector2(0, shortHop);
            fullHopForce = new Vector2(0, fullHop);
            doubleJumpShortHopForce = new Vector2(0, doubleJumpShortHop);
            distanceToWall = GetComponent<Collider2D>().bounds.extents.x+raycastBuffer;
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
                    if (rb.velocity.y > jump*0.75f) {
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
                rb.velocity *= deceleration; 
                return;
            }

            float input = Input.GetAxis("Horizontal");
            if (!isPlatformMomentumEnabled && input != 0) {
                transform.SetParent(null);
            }

            float inputDirection = Mathf.Sign(input);

            // decelerate.
            if (Mathf.Abs(input) != 1 && !isWallJumping) { 
                rb.velocity *= deceleration; 
                return;

            }

            // Get player direction.
            float motionDirection = Mathf.Sign(rb.velocity.x);


            // If the player is turning around, apply more force
            float adjustedInput = inputDirection == motionDirection ? input : input*rebound;
            if (isWallJumping) {
                adjustedInput *= wallJumpAccelerationModifier;
            }

            // calculate new speed of player, accounting for maximum speed.
            if (inputDirection == -1 && isOnLeftWall) {
                rb.velocity = Vector2.up*rb.velocity;
            } else if (inputDirection == 1 && isOnRightWall) {
                rb.velocity = Vector2.up*rb.velocity;
            } else {
                float horizSpeed = Mathf.Clamp(rb.velocity.x+adjustedInput*acceleration, -maxVelocity, maxVelocity);
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
                    jumpRecheckTimer = 0;
                    rb.velocity = rb.velocity*Vector2.right+shortHopForce;

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
                    jumpRecheckTimer = 0;

                    if (isOnLeftWall) {

                        hasJumped = true;
                        canDoubleJump = false;
                        rb.velocity = rb.velocity*Vector2.right+shortHopForce;
                        rb.velocity += wallJumpForce;   
                        isOnLeftWall = false;
                        isWallJumping = true;
                        isInWallJumpCombo = true;

                    } else if (isOnRightWall) {

                        hasJumped = true;
                        canDoubleJump = false;
                        rb.velocity = rb.velocity*Vector2.right+shortHopForce;
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
                jumpRecheckTimer > recheckJump) {
                    
                // Perform a full hop instead of a full hop.
                jumpRecheckTimer = 0;
                rb.velocity = rb.velocity*Vector2.right + fullHopForce;
            }

            jumpRecheckTimer += Time.fixedDeltaTime;
            jumpTimer += Time.fixedDeltaTime;
            if (jumpInputReleased &&
                jumpTimer > jumpInputDelay) {

                if (isOnLeftWall || isOnRightWall) {
                    //Debug.Log("if (isOnLeftWall || isOnRightWall) {");
                    resetJumpLogic();
                } else if (!hasDoubleJumped) {
                    canDoubleJump = true;
                }
            }

        }

        public void resetJumpLogic() {
            // Allow the character 
            jumpTimer = 0;
            jumpRecheckTimer = 0;
            hasJumped = false;
            hasDoubleJumped = false;
            canDoubleJump = false;
            isWallJumping = false;
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
