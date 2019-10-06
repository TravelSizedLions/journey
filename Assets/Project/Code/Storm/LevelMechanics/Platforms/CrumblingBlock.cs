using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.LevelMechanics.Platforms {

    public class CrumblingBlock : MonoBehaviour
    {
        public bool deteriorating;

        public BoxCollider2D physicsCol;
        
        public BoxCollider2D triggerCol;

        public SpriteRenderer sprite;

        public Animator anim;

        // Start is called before the first frame update
        void Start() {
            deteriorating = false;
            anim = GetComponent<Animator>();
            var cols = GetComponents<BoxCollider2D>();
            physicsCol = cols[0];
            triggerCol = cols[1];
            sprite = GetComponent<SpriteRenderer>();
        }


        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                deteriorating = true;
                triggerCol.enabled = false;
            }
        }
    }

}
