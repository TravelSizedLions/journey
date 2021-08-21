using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



namespace HumanBuilders {

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
    /// <param name="autoAdvance">Whether or not to automatically advance the
    /// dialog after typing has finished.</param>
    /// <param name="delayBeforeAdvance">How long to delay before advancing the
    /// dialog, in seconds</param>
    /// <param name="speed">The speed of typing, in characters per second.</param>
    /// <param name="animateSpeaker">Whether or not to control the speaker's animation.</param>
    /// <seealso cref="DialogBox.Type(string, string, bool, float, float, bool)" />
    void Type(string sentence, string speaker = "", bool autoAdvance = false, float delayBeforeAdvance = 0f, float speed = 100f, bool animateSpeaker = true);

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

    /// <summary>
    /// Set the character who should be speaking.
    /// </summary>
    /// <seealso cref="DialogBox.SetCurrentCharacter" />
    void SetCurrentCharacter(CharacterProfile character);

    /// <summary>
    /// Apply character profile information to this dialog box.
    /// </summary>
    /// <param name="profile">The profile to apply.</param>
    /// <seealso cref="DialogBox.ApplyColors" />
    void ApplyColors(Color primary, Color secondary, Color text);

    /// <summary>
    /// Reset the dialog box to its default colors.
    /// </summary>
    void ResetColors();
  }
}