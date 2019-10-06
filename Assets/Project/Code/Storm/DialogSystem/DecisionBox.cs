using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Storm.DialogSystem {
    /**
        TODO: Make a callback to the DialogManager on button press
        TODO: Make a way of dynamically placing a list of these on the screen at a prescribed location.
    */
    public class DecisionBox : MonoBehaviour
    {
        private Button button;
        private TextMeshPro text;

        private int decision;

        // Start is called before the first frame update
        void Start() {
            button = GetComponent<Button>();
            text = button.GetComponent<TextMeshPro>();
        }

        public void SetText(string text) {
            this.text.text = text;
        }

        public void SetDecision(int number) {
            decision = number;
        }
        
        public int GetDecision() {
            return decision;
        }
    }
}


