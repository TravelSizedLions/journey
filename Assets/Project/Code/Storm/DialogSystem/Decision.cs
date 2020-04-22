using System;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.DialogSystem {

  /// <summary>
  /// An option that a player can decide to take in a conversation.
  /// </summary>
  [Serializable]
  public class Decision {
    #region Variables
    /// <summary>
    /// The option displayed to the player.
    /// </summary>
    [Tooltip("The option displayed to the player.")]
    public string OptionText = "";

    /// <summary>
    /// The DialogNode tag the decision leads to.
    /// </summary>
    [Tooltip("The name of the Dialog Node this decision leads to.")]
    public string DestinationTag = "";

    /// <summary> 
    /// Sentences to say after a decision but before 
    /// returning to an earlier dialog.
    /// </summary>
    [Tooltip("Sentences to say after a decision but before returning to an earlier dialog.")]
    public List<Sentence> Consequences;
    #endregion

    #region Constructors
    //---------------------------------------------------------------------
    // Constructor(s)
    //---------------------------------------------------------------------
    public Decision(string optionText, string destinationTag) {
      this.OptionText = optionText;
      this.DestinationTag = destinationTag;
      this.Consequences = new List<Sentence>();
    }

    public Decision(string optionText, string destinationTag, IEnumerable<Sentence> consequences) {
      this.OptionText = optionText;
      this.DestinationTag = destinationTag;
      this.Consequences = new List<Sentence>(consequences);
    }
    #endregion
  }
}