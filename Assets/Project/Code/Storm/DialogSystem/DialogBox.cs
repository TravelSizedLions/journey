using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.DialogSystem {
    public class DialogBox : MonoBehaviour {
        private List<DecisionBox> decisions;

        // Start is called before the first frame update
        void Start() {
            decisions = new List<DecisionBox>();
        }

        // Update is called once per frame
        void Update() {
            
        }

        public void AddDecision(string text, int decision) {
            //decisions.Add();
        }
    }
}
