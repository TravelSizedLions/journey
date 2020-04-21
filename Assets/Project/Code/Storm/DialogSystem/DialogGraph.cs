using System;
using System.Collections.Generic;
using System.Threading;
using Storm.Attributes;
using Storm.Characters.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Storm.DialogSystem {
  /// <summary>
  /// A directed graph representing a conversation the player can have with an NPC.
  /// </summary>
  /// <remarks>
  /// Every conversation is made up of a collection of dialog nodes. 
  /// Each node of dialog is a collection of sentences, possibly followed by a list of decisions the player can make.
  /// Each decision can have a list of sentences that act as a consequence to the decision, and each decision will point to another node on the graph.
  /// 
  /// Each conversation graph also has hooks to unity events that will play at either the beginning or the end of the conversation.
  /// </remarks>
  /// <seealso cref="DialogNode" />
  /// <seealso cref="Sentence" />
  /// <seealso cref="Decision" />
  [Serializable]
  public class DialogGraph : MonoBehaviour {

    #region Variables
    #region Graph Properties
    [Header("The Conversation Graph", order = 0)]
    [Space(5, order = 1)]

    /// <summary> 
    /// The list of nodes in the conversation. 
    /// </summary>
    [Tooltip("The list of nodes in the conversation.")]
    public DialogNode[] nodes;


    /// <summary>
    /// A dictionary mapping names of each part of the dialog to their respective node.
    /// </summary>
    private Dictionary<string, DialogNode> graph;

    /// <summary>
    /// The first set of dialog in a conversation.
    /// </summary>
    private DialogNode root;

    /// <summary>
    /// The current dialog node.
    /// </summary>
    private DialogNode current;

    [Space(15, order = 2)]
    #endregion

    #region Events
    [Header("Conversation Events", order = 3)]
    [Space(5, order = 4)]

    /// <summary> 
    /// The list of events to play at the start of a conversation. 
    /// </summary>
    [Tooltip("The list of events to play at the start of a conversation.")]
    public UnityEvent startEvents;

    /// <summary>
    /// The list of events to play at the end of a conversation. 
    /// </summary>
    [Tooltip("The list of events to play at the end of a conversation.")]
    public UnityEvent closeEvents;

    [Space(15, order = 5)]
    #endregion

    #region Input Collection Variables
    /// <summary> 
    /// Whether or not the player is pressing space. 
    /// </summary>
    [Tooltip("Whether or not the player is pressing space.")]
    [ReadOnly]
    public bool isSpacePressed;
    #endregion
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      if (nodes == null) {
        nodes = new DialogNode[0];
      }

      graph = new Dictionary<string, DialogNode>();
      foreach (DialogNode n in nodes) {
        graph.Add(n.Name, n);
      }

      if (nodes.Length > 0) {
        root = nodes[0];
      }
    }


    private void Update() {
      isSpacePressed = isSpacePressed || Input.GetKeyDown(KeyCode.Space);
    }


    private void OnTriggerEnter2D(Collider2D other) {

      // If the player is in the trigger area
      if (other.CompareTag("Player")) {
        PlayerCharacter player = GameManager.Instance.player;
        player.NormalMovement.DisableJump();
        DialogManager.Instance.AddIndicator();
        DialogManager.Instance.SetCurrentDialog(this);
      }
    }


    private void OnTriggerExit2D(Collider2D other) {
      // If the player has left the trigger area
      if (other.CompareTag("Player") && !DialogManager.Instance.IsInConversation) {
        PlayerCharacter player = GameManager.Instance.player;
        player.NormalMovement.EnableJump();
        DialogManager.Instance.RemoveIndicator();
      }
    }

    #endregion

    #region Public Interfaces
    #region Graph Building Interface
    //---------------------------------------------------------------------
    // Graph Building
    //---------------------------------------------------------------------

    /// <summary>
    /// Add a new Dialog node to the graph.
    /// </summary>
    /// <param name="node">The node to add.</param>
    /// <returns>True if the dialog was added.</returns>
    public bool AddDialog(DialogNode node) {
      if (graph.Count == 0) {
        root = node;
      }
      graph[node.Name] = node;
      return true;
    }

    /// <summary>
    /// Adds a decision point to the graph.
    /// </summary>
    /// <param name="fromTag">The part of the dialog to add a decision to. </param>
    /// <param name="optionText">The wording of the decision. This will be what's displayed to the player as an option.</param>
    /// <param name="toTag">The part of the dialog that this decision leads to.</param>
    public void AddDecision(string fromTag,
      string optionText,
      string toTag) {

      DialogNode fromNode = graph[fromTag];
      fromNode.AddDecision(optionText, toTag);
    }

    /// <summary>
    /// Remove all nodes in the graph.
    /// </summary>
    public void Clear() {
      graph.Clear();
    }
    #endregion

    #region Graph Traversal Interface
    //---------------------------------------------------------------------
    // Graph Traversal
    //---------------------------------------------------------------------

    /// <summary>
    /// Returns the first part of the dialog.
    /// </summary>
    /// <returns></returns>
    public DialogNode GetRootNode() {
      return root;
    }

    /// <summary>
    /// Restarts a dialog graph from the beginning.
    /// </summary>
    /// <returns>The starting dialog node.</returns>
    public DialogNode StartDialog() {
      current = root;
      PerformStartEvents();
      return root;
    }

    /// <summary>
    /// Make a decision that will transition from one part of the dialog to the next.
    /// </summary>
    /// <param name="decision">The decision the player made.</param>
    /// <returns>The next dialog node in the graph.</returns>
    public DialogNode MakeDecision(Decision decision) {
      current = graph[decision.DestinationTag];
      return current;
    }

    /// <summary>
    /// Gets a dialog node by it's name.
    /// </summary>
    /// <param name="nodeName">The name of the dialog node.</param>
    /// <returns>The dialog node.</returns>
    public DialogNode GetNode(string nodeName) {
      if (graph.ContainsKey(nodeName)) {
        return graph[nodeName];
      }

      throw new UnityException("No such dialog node: '" + nodeName + "'");
    }

    /// <summary>
    /// Get the current node in the dialog graph.
    /// </summary>
    /// <returns>The current dialog node.</returns>
    public DialogNode GetCurrentDialog() {
      return current;
    }

    /// <summary>
    /// Determines whether or not the conversation is finished.
    /// </summary>
    /// <returns>True if there are no more decisions the player can make, and the current node doesn't transition to another node. False otherwise.</returns>
    public bool IsFinished() {
      if (current.Decisions != null) {
        return current.Decisions.Count == 0 &&
          (current.NextNode == "" || current.NextNode == null);
      } else {
        return true;
      }
    }
    #endregion

    #region Event Handling Interface
    //---------------------------------------------------------------------
    // Event Handling
    //---------------------------------------------------------------------
    /// <summary>
    /// Whether or not the conversation graph has events to play at the start of the conversation.
    /// </summary>
    public bool HasStartEvents() {
      return (startEvents.GetPersistentEventCount() > 0);
    }

    /// <summary>
    /// Performs all events registered for the start of the conversation.
    /// </summary>
    public void PerformStartEvents() {
      startEvents.Invoke();
    }

    /// <summary>
    /// Whether or not the conversation graph has events to play at the end of the conversation.
    /// </summary>
    public bool HasCloseEvents() {
      return (closeEvents.GetPersistentEventCount() > 0);
    }

    /// <summary>
    /// Performs all events registered for the end of the conversation.
    /// </summary>
    public void PerformCloseEvents() {
      closeEvents.Invoke();
    }
    #endregion
    #endregion
  }
}