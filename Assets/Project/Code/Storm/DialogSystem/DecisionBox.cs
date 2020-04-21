using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Storm.DialogSystem {
  /**
      TODO: Make a callback to the DialogManager on button press
      TODO: Make a way of dynamically placing a list of these on the screen at a prescribed location.
  */
  /// <summary>
  /// A UI element for displaying potential decisions a player could make in a dialog.
  /// </summary>
  public class DecisionBox : MonoBehaviour {

    #region Variables
    /// <summary>
    /// The element the player can press to make the decision.
    /// </summary>
    [ReadOnly]
    public Button ButtonElement;

    /// <summary>
    /// The text element for displaying the text of the decision.
    /// </summary>
    [ReadOnly]
    public TextMeshProUGUI TextElement;

    /// <summary>
    /// Which decision in a list that this DecisionBox represents. 
    /// </summary>
    [ReadOnly]
    public int Decision;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      ButtonElement = GetComponent<Button>();
      TextElement = ButtonElement.GetComponentInChildren<TextMeshProUGUI>();
    }

    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Set the text displayed to the player.
    /// </summary>
    /// <param name="text">The text to display.</param>
    public void SetText(string text) {
      this.TextElement.text = text;
    }

    /// <summary>
    /// Sets which decision this element is supposed to represent on screen.
    /// </summary>
    /// <param name="decision">The index in a list of decisions.</param>
    public void SetDecision(int decision) {
      this.Decision = decision;
    }

    /// <summary>
    /// Gets the decision this element is supposed to represent on screen.
    /// </summary>
    public int GetDecision() {
      return Decision;
    }
    #endregion
  }
}