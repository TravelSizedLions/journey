using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;


namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A collection of snippets leading up to a decision point.
  /// </summary>
  /// <remarks>
  /// The DialogManager will run through all dialog snippets placed
  /// on this node and then present the user with the list of decisions.
  /// </remarks>
  /// <seealso cref="DialogManager"/>
  [Serializable]
  public class DialogNode {

    #region Variables
    /// <summary>
    /// A tag that identifies the node within the graph.
    /// This does not need to be unique to the whole game,
    /// just to the particular in-game conversation.
    /// </summary>
    [Tooltip("The name of the node withing the graph. This needs to be unique to the conversation.")]
    public string Name;

    /// <summary> 
    /// The list of sentences to run through. 
    /// </summary>
    [Tooltip("The list of sentences in this part of the dialog.")]
    public List<Sentence> Sentences;

    /// <summary> 
    /// The list of decisions to make. 
    /// </summary>
    [Tooltip("The list of decisions the player can make.")]
    public List<Decision> Decisions;

    /// <summary>
    /// The name of the next node to traverse to.
    /// </summary>
    [Tooltip("The conversation node to play after this one (if there are no decisions the player can make).")]
    public string NextNode;
    #endregion

    #region Constructors
    //---------------------------------------------------------------------
    // Constructors
    //---------------------------------------------------------------------

    public DialogNode(string tag) {
      this.Name = tag;
      Sentences = new List<Sentence>();
      Decisions = new List<Decision>();
    }

    public DialogNode(string tag,
      IEnumerable<Sentence> snippets,
      IEnumerable<Decision> decisions) {

      this.Name = tag;
      this.Sentences = new List<Sentence>(snippets);
      this.Decisions = new List<Decision>(decisions);
    }

    public DialogNode(string tag, IEnumerable<Sentence> snippets) {
      this.Name = tag;
      this.Sentences = new List<Sentence>(snippets);
      Decisions = new List<Decision>();
    }
    #endregion

    #region Graph Building
    //---------------------------------------------------------------------
    // Graph Building
    //---------------------------------------------------------------------

    /// <summary>
    /// Add a sentence.
    /// </summary>
    /// <param name="sentence">The sentence to add.</param>
    public void AddSentence(Sentence sentence) {
      Sentences.Add(sentence);
    }

    /// <summary>
    /// Add a sentence.
    /// </summary>
    /// <param name="speaker">The name of the person speaking.</param>
    /// <param name="sentence">The sentence to say.</param>
    /// <returns></returns>
    public Sentence AddSentence(string speaker, string sentence) {
      Sentence snippet = new Sentence(speaker, sentence);
      Sentences.Add(snippet);
      return snippet;
    }

    /// <summary>
    /// Add a decision.
    /// </summary>
    /// <param name="decision">The decision to add.</param>
    public void AddDecision(Decision decision) {
      Decisions.Add(decision);
    }

    /// <summary>
    /// Add a decision.
    /// </summary>
    /// <param name="optionText">The text of the option. This is what will be displayed to the player.</param>
    /// <param name="destinationTag">The name of the node this decision should lead to.</param>
    /// <returns>The decision that was added.</returns>
    public Decision AddDecision(string optionText, string destinationTag) {
      Decision transition = new Decision(optionText, destinationTag);
      Decisions.Add(transition);
      return transition;
    }

    /// <summary>
    /// Clear the list of sentences.
    /// </summary>
    public void ClearSentences() {
      Sentences.Clear();
    }

    /// <summary>
    /// Clear the list of decisions.
    /// </summary>
    public void ClearDecisions() {
      Decisions.Clear();
    }
    #endregion
  }
}