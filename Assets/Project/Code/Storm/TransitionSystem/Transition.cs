using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.TransitionSystem {
    public class Transition : MonoBehaviour {

        public string destinationScene;

        public string spawnPointName;

        public string vcamName;

        private TransitionManager transitions;
        void Start(){
            transitions = GameManager.Instance.transitions;
        }

        public void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                //Debug.Log("Loading new scene!");
                transitions.MakeTransition(destinationScene, spawnPointName, vcamName);
            }
        }
    }

}
