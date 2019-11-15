using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Storm.Flexible {

    public class Invisible : MonoBehaviour {
        void Awake() {
            var sprite = GetComponent<SpriteRenderer>();
            if (sprite != null) {
                sprite.enabled = false;
            }

            foreach (var child in GetComponentsInChildren<SpriteRenderer>(true)) {
                child.enabled = false;
            }


            var image = GetComponent<Image>();
            if (image != null) {
                image.enabled = false;
            }

            foreach (var child in GetComponentsInChildren<Image>(true)) {
                child.enabled = false;
            }
        }
    }
}