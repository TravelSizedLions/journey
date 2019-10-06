using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Storm.DialogSystem {
    /*
        The most basic unit of dialog. Think of a
        snippet as a single window of text.
    */
    [Serializable]
    public class Sentence {


        // The person saying the sentence.
        public string speaker;

        // The sentence being said.
        [TextArea(3,10)]
        public string sentence;

        public UnityEvent events;
        
        //---------------------------------------------------------------------
        // Constructor(s)
        //---------------------------------------------------------------------

        public Sentence(string speaker, string sentence) {
            this.speaker = speaker;
            this.sentence = sentence;
        }

        public bool HasEvents() {
            return events.GetPersistentEventCount() > 0;
        }

        public void PerformEvents() {
            events.Invoke();
        }
    }
}
