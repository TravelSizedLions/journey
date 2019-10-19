using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
    public class LiveWireMovement : PlayerBehavior  {

        #region Movement Parameters
        [Header("Movement Parameters", order=0)]
        [Space(5, order=1)]

        [Tooltip("How fast the player zips from point to point.")]
        public float topSpeed;

        /// <summary>
        /// Calculated squared velocity from topSpeed.
        /// </summary>
        protected float maxSqrVelocity;

        [Tooltip("How fast the player reaches top speed. 0 - no movement, 1 - instantaneous top speed")]
        [Range(0,1)]
        public float acceleration;

        /// <summary>
        /// The target player velocity. Calculated from direction and topSpeed.
        /// </summary>
        private Vector2 targetVelocity;

        /// <summary>
        /// Sets how big of a jump the player performs upon exiting LiveWire mode.
        /// </summary>
        [Tooltip("Sets how big of a jump the player performs upon exiting LiveWire mode.")]
        public float postTransitJump;

        /// <summary>
        /// The jump force vector calculated from the jump variable.
        /// </summary>
        protected Vector2 jumpForce;

        [Space(15, order=2)]
        #endregion
        

        #region Appearance Parameters
        [Header("Appearance Parameters", order=3)]
        [Space(5, order=4)]

        /// <summary>
        /// How much the spark stretches. Higher = more stretch.
        /// </summary>
        [Tooltip("How much the spark stretches. Higher = more stretch.")]
        public float stretch;

        /// <summary>
        /// How thick the spark should be.
        /// </summary>
        [Tooltip("How thick the spark should be.")]
        public float thickness;

        /// <summary>
        /// The target scale calculated from stretch and thickness.
        /// </summary>
        private Vector3 scale;
        #endregion


        public override void Awake() {
            base.Awake();
        }

        public void Start() {
            jumpForce = new Vector2(0, postTransitJump);
            maxSqrVelocity = topSpeed*topSpeed;
            rb.freezeRotation = true;
            scale = new Vector3(stretch, thickness, 1);
        }


        public void Update() {
            transform.localScale = Vector3.Lerp(transform.localScale,scale,acceleration);
        }


        public void FixedUpdate() {
            rb.velocity = Vector2.Lerp(rb.velocity,targetVelocity,acceleration);
        }

        public void SetDirection(Vector2 direction) {
            rb.velocity = Vector2.zero;
            targetVelocity = direction*topSpeed;
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(1, thickness, 1);

            transform.Rotate(0,0,Vector2.SignedAngle(Vector2.right, direction));
        }

        /// <summary>
        /// Called every time the player switches to Directed LiveWire Movement.
        /// </summary>
        public override void Activate() {
            if (!enabled) {
                base.Activate();
                if (rb == null) {
                    rb = GetComponent<Rigidbody2D>();
                }
                rb.velocity = Vector2.zero;
                foreach(var param in anim.parameters) {            
                    anim.SetBool(param.name, false);            
                }
                rb.gravityScale = 0;
                gameObject.layer = LayerMask.NameToLayer("LiveWire");
                anim.SetBool("LiveWire",true);
            }
        }

        /// <summary>
        /// Called every time the player switches away from Directed LiveWire Movement.
        /// </summary>
        public override void Deactivate() {
            if (enabled) {
                base.Deactivate();
                anim.SetBool("LiveWire", false);
                rb.velocity += jumpForce;
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                rb.gravityScale = 1;
                gameObject.layer = LayerMask.NameToLayer("Player");
            }

        }
    }
}