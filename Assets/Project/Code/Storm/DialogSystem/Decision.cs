using System.Collections.Generic;
using System;
using UnityEngine;

namespace Storm.DialogSystem {

    /*
        An option a player can decide to take in a conversation.
    */
    [Serializable]
    public class Decision {
        // The text displayed to the player.
        public string optionText;

        // The DialogNode tag the decision leads to.
        public string destinationTag;

        // Snippets to play after a decision but before 
        // returning to an earlier dialog.
        public List<Sentence> consequences;

        //---------------------------------------------------------------------
        // Constructor(s)
        //---------------------------------------------------------------------

        public Decision(string optionText, string destinationTag) {
            this.optionText = optionText;
            this.destinationTag = destinationTag;
            this.consequences = new List<Sentence>();
        }

        public Decision(string optionText, string destinationTag, IEnumerable<Sentence> consequences) {
            this.optionText = optionText;
            this.destinationTag = destinationTag;
            this.consequences = new List<Sentence>(consequences);
        }

    }
}