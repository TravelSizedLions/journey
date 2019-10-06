using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
    public class LiveWireMovement : PlayerMovement  {


        public float stretch;

        public float thickness;

        private Vector3 scale;

        private Vector2 targetVelocity;

        public override void Start() {
            anim = GetComponent<Animator>();

            jumpForce = new Vector2(0, jump);
            maxSqrVelocity = maxVelocity*maxVelocity;
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;

            sensor = GetComponent<PlayerCollisionSensor>();

            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
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
            targetVelocity = direction*maxVelocity;
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(1, thickness, 1);

            transform.Rotate(0,0,Vector2.SignedAngle(Vector2.right, direction));
        }

        public override void Deactivate() {
            base.Deactivate();
            if (anim == null) {
                anim = GetComponent<Animator>();
            }
            anim.SetBool("LiveWire", false);
            if (rb == null) {
                rb = GetComponent<Rigidbody2D>();
            }
            rb.velocity += jumpForce;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            rb.gravityScale = 1;
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        public override void Activate() {
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
}