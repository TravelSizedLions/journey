using UnityEngine;
using Storm.Extensions;
using Storm.Characters.Player;
using Storm.Subsystems.Dialog;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// This class is responsible for traversing over XNode graphs.
  /// </summary>
  /// <remarks>
  /// Other classes can wrap around this subsystem to provide more specific
  /// behavior/APIs (see the DialogManager).
  /// </remarks>
  /// <seealso cref="DialogManager" />
  /// <seealso cref="Storm.Characters.Bosses.Boss" />
  public class GraphEngine : MonoBehaviour {
    #region Delegates
    //-------------------------------------------------------------------------
    // Fields
    //------------------------------------------------------------------------
    /// <summary>
    /// This delegate gets fired when GraphEngine.EndGraph() is called.
    /// </summary>
    public delegate void GraphEnded();
    public GraphEnded OnGraphEnded;
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The current conversation being played out.
    /// </summary>
    private IAutoGraph currentGraph;

    /// <summary>
    /// The current dialog node.
    /// </summary>
    private IAutoNode currentNode;
    
    /// <summary>
    /// Whether or not the manager is currently busy managing a node in the conversation.
    /// </summary>
    [Tooltip("Whether or not the manager is currently busy managing the conversation.")]
    [SerializeField]
    [ReadOnly]
    public bool handlingNode;

    /// <summary>
    /// Whether or not the current node in the dialog has locked progress in the converation.
    /// </summary>
    [Tooltip("Whether or not the current node in the dialog has locked progress in the converation.")]
    [SerializeField]
    [ReadOnly]
    private bool nodeLocked;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Have the current node check any registered conditions each frame.
    /// </summary>
    private void Update() {
      if (currentNode != null) {
        currentNode.CheckConditions();
      }
    }

    /// <summary>
    /// Delegate function for MonoBehavior's StartCoroutine(). :)
    /// </summary>
    /// <param name="routine">The routine to run.</param>
    public void StartThread(IEnumerator routine) => StartCoroutine(routine);

    #endregion

    #region Dependency Injection
    //-------------------------------------------------------------------------
    // Dependency Injection
    //-------------------------------------------------------------------------
    /// <summary>
    /// Inject a graph (used for automated testing -- for normal dev, use
    /// <see cref="GraphEngine.SetCurrentGraph" /> instead).
    /// </summary>
    public void Inject(IAutoGraph graph) {
      this.currentGraph = graph;
    }

    /// <summary>
    /// Inject a node to be the current node (used for automated testing -- for
    /// normal dev, use <see cref="GraphEngine.SetCurrentNode" /> instead).
    /// </summary>
    public void Inject(IAutoNode node) {
      this.currentNode = node;
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Begin traversing a new graph.
    /// </summary>
    public void StartGraph(IAutoGraph graph) {
      if (!handlingNode) {
        handlingNode = true;

        currentGraph = graph;
        currentNode = currentGraph.FindStartingNode();

        handlingNode = false;
        if (currentNode == null) {
          return;
        }

        Continue();
      }
    }

    /// <summary>
    /// Continue traversing the current graph.
    /// </summary>
    public void Continue() => currentNode.HandleNode(this);

    /// <summary>
    /// Finish traversing the current graph.
    /// </summary>
    public void EndGraph() {
      UnlockNode();
      FinishHandlingNode();

      currentGraph = null;
      currentNode = null;

      // Perform any callbacks registered to this engine.
      if (OnGraphEnded != null) {
        OnGraphEnded();
      }
    }

    /// <summary>
    /// Set the current node for the graph. Don't use this while in the middle
    /// of traversing another graph.
    /// </summary>
    public void SetCurrentNode(IAutoNode node) => currentNode = node;
    
    /// <summary>
    /// Get the current node in the graph.
    /// </summary>
    public IAutoNode GetCurrentNode() => currentNode;

    /// <summary>
    /// Set the current graph to be traversed.
    /// Don't use this while in the middle of travering another graph.
    /// </summary>
    public void SetCurrentGraph(IAutoGraph graph) => currentGraph = graph;

    /// <summary>
    /// Get the graph that's currently being traversed.
    /// </summary>
    public IAutoGraph GetCurrentGraph() => currentGraph;

    /// <summary>
    /// Whether or not the graph is finished being traversed.
    /// </summary>
    public bool IsGraphFinished() => (currentGraph == null);

    /// <summary>
    /// Locks handling a dialog. This will prevent more nodes from being fired
    /// in a conversation until the lock has been released.
    /// </summary>
    /// <returns>True if the lock was obtained, false otherwise.</returns>
    public bool LockNode() {
      if (nodeLocked) {
        return false;
      }

      nodeLocked = true;
      return true;
    }

    /// <summary>
    /// Unlocks handling a dialog. If there was previously a lock on firing more
    /// nodes in the conversation, this will release it.
    /// </summary>
    /// <returns>
    /// Whether or not the current node was locked.
    /// </returns>
    /// <remarks>
    /// Don't use this without first trying to obtain the lock for yourself.
    /// </remarks>
    public bool UnlockNode() {
      bool result = nodeLocked;
      nodeLocked = false;
      return nodeLocked;
    }

    /// <summary>
    /// Try to start handling a node in the conversation.
    /// </summary>
    /// <returns>
    /// True if previous node in the conversation graph is finished being handled. False otherwise.
    /// </returns>
    public bool StartHandlingNode() {
      if (!nodeLocked) {
        handlingNode = true;
        return true;
      } else {
        return false;
      }
    }

    /// <summary>
    /// Try to finish handling a node in the conversation.
    /// </summary>
    /// <returns>
    /// True if the current node finished handling successfully. False if the current node still needs time to finish.
    /// </returns>
    public bool FinishHandlingNode() {
      if (!nodeLocked) {
        handlingNode = false;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}