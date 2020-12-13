using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Storm;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A class representing a Dialog Box UI element.
  /// </summary>
  public interface IDialogBox : IMonoBehaviour {

    /// <summary>
    /// Open the dialog box.
    /// </summary>
    /// <seealso cref="DialogBox.Open" />
    void Open();

    /// <summary>
    /// Close the dialog box.
    /// </summary>
    /// <seealso cref="DialogBox.Close" />
    void Close();

    /// <summary>
    /// Type out a sentence spoken by a certain speaker. If another sentence is
    /// already being typed, the dialog box will instead skip to the end of the
    /// sentence being typed. If the player will need to make a decision
    /// afterward, this will also display the options.
    /// </summary>
    /// <param name="sentence">The sentence to type.</param>
    /// <param name="speaker">The speaker of the sentence.</param>
    /// <param name="speed">The speed of typing, in characters per second.</param>
    /// <seealso cref="DialogBox.Type" />
    void Type(string sentence, string speaker = "", float speed = 100f);

    /// <summary>
    /// Clears the sentence text on the dialog box.
    /// </summary>
    /// <seealso cref="DialogBox.ClearText" />
    void ClearText();

    /// <summary>
    /// Remove the decision buttons from the screen.
    /// </summary>
    /// <seealso cref="DialogBox.ClearDecisions" />
    void ClearDecisions();

    /// <summary>
    /// Gets the list of active decisions on screen.
    /// </summary>
    /// <seealso cref="DialogBox.GetDecisionButtons" />
    List<DecisionBox> GetDecisionButtons();
  }
}