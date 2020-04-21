using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Storm.DialogSystem {
  /// <summary>
  /// The most basic unit of dialog. Think of a
  /// sentence as a single window of text.
  /// </summary>
  [Serializable]
  public class Sentence {

    /// <summary> 
    /// The person saying the sentence. 
    /// </summary>
    [Tooltip("The person saying the sentence.")]
    public string Speaker;

    /// <summary>
    /// The sentence being said.
    /// </summary>
    [TextArea(3, 10)]
    [Tooltip("The sentence to say.")]
    public string SentenceText;

    /// <summary>
    /// Any events that should fire when this sentence plays.
    /// </summary>
    [Tooltip("Events that will fire when this sentence plays.")]
    public UnityEvent Events;

    #region Constructors
    //---------------------------------------------------------------------
    // Constructor(s)
    //---------------------------------------------------------------------

    public Sentence(string speaker, string sentence) {
      this.Speaker = speaker;
      this.SentenceText = sentence;
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------


    /// <summary>
    /// Whether or not this sentence has unity events subscribed to it.
    /// </summary>
    /// <returns>True if there are events subscribed, false otherwise.</returns>
    public bool HasEvents() {
      return Events.GetPersistentEventCount() > 0;
    }

    /// <summary>
    /// Performs the events associated with this sentence.
    /// </summary>
    public void PerformEvents() {
      Events.Invoke();
    }

    #endregion
  }
}