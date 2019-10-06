using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Flexible {

    public class Invisible : MonoBehaviour {
        void Start() {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}