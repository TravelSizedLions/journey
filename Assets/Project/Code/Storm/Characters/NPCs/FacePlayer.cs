using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;

namespace Storm.Characters.NPCs {
    public class FacePlayer : MonoBehaviour {
        private SpriteRenderer sprite;

        private static PlayerCharacter player;

        // Start is called before the first frame update
        void Start() {
            sprite = GetComponent<SpriteRenderer>();
            player = FindObjectOfType<PlayerCharacter>();
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.x > player.transform.position.x && !sprite.flipX) {
                sprite.flipX = true;
                transform.localScale.Set(-1, transform.localScale.y, transform.localScale.z);
            } else if (transform.position.x <= player.transform.position.x && sprite.flipX) {
                sprite.flipX = false;
                transform.localScale.Set(1, transform.localScale.y, transform.localScale.z);
            }
        }
    }
}

