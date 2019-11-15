using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Storm.Attributes;

namespace Storm.DialogSystem {
    /**
        TODO: Make a callback to the DialogManager on button press
        TODO: Make a way of dynamically placing a list of these on the screen at a prescribed location.
    */
    public class DecisionBox : MonoBehaviour
    {
        [ReadOnly]
        public Button button;

        [ReadOnly]
        public TextMeshProUGUI text;

        [ReadOnly]
        public int decision;

        // Start is called before the first frame update
        void Awake() {
            button = GetComponent<Button>();
            text = button.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetText(string text) {
            this.text.text = text;
        }

        public void SetDecision(int decision) {
            this.decision = decision;
        }
        
        public int GetDecision() {
            return decision;
        }
    }
}


