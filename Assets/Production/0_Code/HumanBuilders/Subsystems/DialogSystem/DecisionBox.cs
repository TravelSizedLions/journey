using System.Collections;
using System.Collections.Generic;
using HumanBuilders.Graphing;
using Sirenix.OdinInspector;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HumanBuilders {
  /// <summary>
  /// A UI element for displaying potential decisions a player could make in a dialog.
  /// </summary>
  /// <remarks>
  /// This is a class for the UI, not the underlying model for a decision. To see that, check out Decision.cs
  /// </remarks>
  /// <seealso cref="HumanBuildersOld.Decision" />
  public class DecisionBox : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The list of elements that are colored with the primary color.
    /// </summary>
    [Tooltip("The list of elements that are colored with the primary color.")]
    public List<Image> PrimaryColorComponents;

    /// <summary>
    /// The list of elements that are colored with the secondary color.
    /// </summary>
    [Tooltip("The list of elements that are colored with the secondary color.")]
    public List<Image> SecondaryColorComponents;
    
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

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      ButtonElement = GetComponent<Button>();
      TextElement = ButtonElement.GetComponentInChildren<TextMeshProUGUI>();
      ButtonElement.onClick.AddListener(OnClick);
    }

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

    public void ApplyColors(Color primary, Color secondary, Color text) {
      ApplyPrimaryColor(primary);
      ApplySecondaryColor(secondary);
      ApplyTextColor(text);
    }

    private void ApplyPrimaryColor(Color color) {
      foreach (Image image in PrimaryColorComponents) {
        image.color = color;
      }
    }

    private void ApplySecondaryColor(Color color) {
      foreach (Image image in SecondaryColorComponents) {
        image.color = color;
      }
    }

    private void ApplyTextColor(Color color) {
      TextElement.color = color;
    }

    public void OnClick() {
      ((DecisionNode)DialogManager.GetCurrentNode()).Decide(Decision, DialogManager.GraphEngine);
      PlayerCharacter player = GameManager.Player;
      if (DialogManager.IsDialogFinished()) {
        player.EndInteraction();
      }
    }
  }
}
