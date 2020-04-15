using System;
using System.Collections.Generic;
using UnityEngine.Events;


namespace Storm.DialogSystem {

  /// <summary>A collection of snippets leading up to a decision point.</summary>
  /// <remarks>
  /// The DialogManager will run through all dialog snippets placed
  /// on this node and then present the user with the list of decisions.
  /// </remarks>
  [Serializable]
  public class DialogNode {

    ///<summary>
    /// A tag that identifies the node within the graph.
    /// This does not need to be unique to the whole game,
    /// just to the particular in-game conversation.
    /// </summary>
    public string name;

    /// <summary> The list of dialog snippets to run through. </summary>
    public List<Sentence> snippets;

    /// <summary> The list of decisions to make. </summary>
    public List<Decision> decisions;

    /// <summary>
    /// The name of the next node to traverse to.
    /// </summary>
    public string nextNode;

    //---------------------------------------------------------------------
    // Constructors
    //---------------------------------------------------------------------

    public DialogNode(string tag) {
      this.name = tag;
      snippets = new List<Sentence>();
      decisions = new List<Decision>();
    }

    public DialogNode(string tag,
      IEnumerable<Sentence> snippets,
      IEnumerable<Decision> decisions) {

      this.name = tag;
      this.snippets = new List<Sentence>(snippets);
      this.decisions = new List<Decision>(decisions);
    }

    public DialogNode(string tag, IEnumerable<Sentence> snippets) {
      this.name = tag;
      this.snippets = new List<Sentence>(snippets);
      decisions = new List<Decision>();
    }



    //---------------------------------------------------------------------
    // Graph Building
    //---------------------------------------------------------------------

    public void AddSnippet(Sentence snippet) {
      snippets.Add(snippet);
    }

    public Sentence AddSnippet(string speaker, string sentence) {
      Sentence snippet = new Sentence(speaker, sentence);
      snippets.Add(snippet);
      return snippet;
    }

    public void AddDecision(Decision transition) {
      decisions.Add(transition);
    }

    public Decision AddDecision(string optionText, string destinationTag) {
      Decision transition = new Decision(optionText, destinationTag);
      decisions.Add(transition);
      return transition;
    }

    public void ClearSnippets() {
      snippets.Clear();
    }

    public void ClearDecisions() {
      decisions.Clear();
    }

  }
}