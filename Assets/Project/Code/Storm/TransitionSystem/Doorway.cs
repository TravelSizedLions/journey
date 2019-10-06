using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;

namespace Storm.TransitionSystem {
    public class Doorway : MonoBehaviour {
        public string sceneName;
        public string spawnName;

        public void OnTriggerStay2D(Collider2D collider) {
            if(collider.CompareTag("Player")) {
                if(Input.GetKeyDown(KeyCode.UpArrow)) {
                    var manager = GameManager.Instance;
                    manager.transitions.MakeTransition(sceneName, spawnName);
                }
            }
        }
    }
}