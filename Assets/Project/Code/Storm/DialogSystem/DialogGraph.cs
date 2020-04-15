using System;
using System.Collections.Generic;
using System.Threading;
using Storm.Attributes;
using Storm.Characters.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Storm.DialogSystem {
  [Serializable]
  public class DialogGraph : MonoBehaviour {

    #region Graph Properties
    [Header("The Conversation Graph", order = 0)]
    [Space(5, order = 1)]

    /// <summary> The full list of nodes in the conversation. </summary>
    [Tooltip("The full conversation.")]
    public DialogNode[] nodes;


    /// <summary>
    /// The graph 
    /// </summary>
    private Dictionary<string, DialogNode> graph;


    // The first set of dialog in a conversation.
    private DialogNode root;

    // The current dialog node.
    private DialogNode current;

    // TODO: Make a way to import/export to XML
    // public string file;

    [Space(15, order = 2)]
    #endregion


    #region Events
    [Header("Conversation Events", order = 3)]
    [Space(5, order = 4)]

    /// <summary> The list of events to play at the start of a conversation. </summary>
    [Tooltip("The list of events to play at the start of a conversation.")]
    public UnityEvent startEvents;

    /// <summary> The list of events to play at the end of a conversation. </summary>
    [Tooltip("The list of events to play at the end of a conversation.")]
    public UnityEvent closeEvents;

    [Space(15, order = 5)]
    #endregion

    /// <summary> Whether or not the player is pressing space. </summary>
    [Tooltip("Whether or not the player is pressing space.")]
    [ReadOnly]
    public bool isSpacePressed;



    //---------------------------------------------------------------------
    // Constructor(s)
    //---------------------------------------------------------------------
    public void Awake() {
      if (nodes == null) {
        nodes = new DialogNode[0];
      }

      graph = new Dictionary<string, DialogNode>();
      foreach (DialogNode n in nodes) {
        graph.Add(n.name, n);
      }

      if (nodes.Length > 0) {
        root = nodes[0];
      }
    }


    public void Update() {
      isSpacePressed = isSpacePressed || Input.GetKeyDown(KeyCode.Space);
    }



    //---------------------------------------------------------------------
    // Graph Building
    //---------------------------------------------------------------------

    public bool AddDialog(DialogNode node) {
      if (graph.Count == 0) {
        root = node;
      }
      graph[node.name] = node;
      return true;
    }

    // Add a transition from one Dialog node to another.
    public void AddTransition(string fromTag,
      string optionText,
      string toTag) {

      DialogNode fromNode = graph[fromTag];
      fromNode.AddDecision(optionText, toTag);
    }

    public void Clear() {
      graph.Clear();
    }



    //---------------------------------------------------------------------
    // Graph Traversal
    //---------------------------------------------------------------------

    public DialogNode GetRootNode() {
      return root;
    }

    public DialogNode StartDialog() {
      current = root;
      PerformStartEvents();
      return root;
    }

    public DialogNode MakeDecision(Decision decision) {
      current = graph[decision.destinationTag];
      return current;
    }

    public DialogNode GetNode(string nodeName) {
      if (graph.ContainsKey(nodeName)) {
        return graph[nodeName];
      }

      throw new UnityException("No such dialog node: '" + nodeName + "'");
    }

    public DialogNode GetCurrentDialog() {
      return current;
    }

    public bool IsFinished() {
      if (current.decisions != null) {
        return current.decisions.Count == 0 &&
          (current.nextNode == "" || current.nextNode == null);
      } else {
        return true;
      }
    }



    //---------------------------------------------------------------------
    // Event Handling
    //---------------------------------------------------------------------
    public bool HasStartEvents() {
      return (startEvents.GetPersistentEventCount() > 0);
    }

    public void PerformStartEvents() {
      startEvents.Invoke();
    }

    public bool HasCloseEvents() {
      return (closeEvents.GetPersistentEventCount() > 0);
    }

    public void PerformCloseEvents() {
      closeEvents.Invoke();
    }



    //---------------------------------------------------------------------
    // Dialog Triggering
    //---------------------------------------------------------------------

    public void OnTriggerEnter2D(Collider2D other) {

      // If the player is in the trigger area
      if (other.CompareTag("Player")) {
        PlayerCharacter player = GameManager.Instance.player;
        player.normalMovement.DisableJump();
        InGameDialogManager.Instance.AddIndicator();
        InGameDialogManager.Instance.SetCurrentDialog(this);
      }
    }


    public void OnTriggerExit2D(Collider2D other) {
      // If the player has left the trigger area
      if (other.CompareTag("Player") && !InGameDialogManager.Instance.isInConversation) {
        PlayerCharacter player = GameManager.Instance.player;
        player.normalMovement.EnableJump();
        InGameDialogManager.Instance.RemoveIndicator();
      }
    }


  }
}