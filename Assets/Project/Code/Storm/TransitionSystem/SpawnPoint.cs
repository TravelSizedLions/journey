using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.TransitionSystem {
    public class SpawnPoint : MonoBehaviour {

        public bool isFacingRight;

        // Start is called before the first frame update
        void Awake() {
            GameManager.Instance.transitions.RegisterSpawn(this.name, transform.position, isFacingRight);
        }

    }
}

