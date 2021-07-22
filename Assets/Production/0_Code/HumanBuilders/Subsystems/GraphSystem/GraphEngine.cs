using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  /// <summary>
  /// This class is responsible for traversing over XNode graphs.
  /// </summary>
  /// <remarks>
  /// Other classes can wrap around this subsystem to provide more specific
  /// behavior/APIs (see the DialogManager).
  /// </remarks>
  /// <seealso cref="DialogManager" />
  /// <seealso cref="HumanBuilders.Boss" />
  public class GraphEngine : MonoBehaviour, IObservable<GraphInfo> {

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Whether or not to enforce depth-first traversal of graphs. If false,
    /// graphs are traversed in breadth-first order instead.
    /// </summary>
    public bool DepthFirst;

    /// <summary>
    /// The current conversation being played out.
    /// </summary>
    private IAutoGraph currentGraph;

    /// <summary>
    /// The list of nodes next up to be handled.
    /// </summary>
    private List<IAutoNode> currentNodes;

    /// <summary>
    /// Nodes that are currently queued up to be handled. This queue is used to
    /// enforce breadth-first traversal.
    /// </summary>
    private Queue<IAutoNode> handlerQueue;

    /// <summary>
    /// Nodes that are currently queued up to be handled. This queue is used to
    /// enforce depth-first traversal.
    /// </summary>
    private Stack<IAutoNode> handlerStack;

    /// <summary>
    /// Whether or not the manager is currently busy managing a node in the conversation.
    /// </summary>
    [Tooltip("Whether or not the manager is currently busy managing the conversation.")]
    [SerializeField]
    [ReadOnly]
    private bool handlingNode;

    /// <summary>
    /// Whether or not the current node in the dialog has locked progress in the converation.
    /// </summary>
    [Tooltip("Whether or not the current node in the dialog has locked progress in the converation.")]
    [SerializeField]
    [ReadOnly]
    private bool nodeLocked;

    /// <summary>
    /// The observers for this graph engine.
    /// </summary>
    private List<IObserver<GraphInfo>> continueObservers;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      currentNodes = new List<IAutoNode>();

      continueObservers = new List<IObserver<GraphInfo>>();
      handlerQueue = new Queue<IAutoNode>();
      handlerStack = new Stack<IAutoNode>();
    }

    /// <summary>
    /// Have the current node check any registered conditions each frame.
    /// </summary>
    private void Update() {
      if (currentNodes != null) {
        // It's possible currentNodes may get modified mid update. For now, just
        // make a copy of the nodes as-is.
        List<IAutoNode> snapshot = new List<IAutoNode>(currentNodes.ToArray());
        foreach (IAutoNode node in snapshot) {
          node.CheckConditions();
        }
      }
    }

    /// <summary>
    /// Delegate function for MonoBehavior's StartCoroutine(). :)
    /// </summary>
    /// <param name="routine">The routine to run.</param>
    public void StartThread(IEnumerator routine) => StartCoroutine(routine);

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
      currentNodes = currentNodes ?? new List<IAutoNode>();
      currentNodes.Clear();
      currentNodes.Add(node);
    }

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
        currentNodes = currentNodes ?? new List<IAutoNode>();
        currentNodes.Clear();

        IAutoNode node = currentGraph.FindStartingNode();

        handlingNode = false;
        if (node == null) {
          return;
        }

        currentNodes.Add(node);
        Continue();
      }
    }

    /// <summary>
    /// Continue traversing the current graph.
    /// </summary>
    public void Continue() {
      if (currentNodes != null) {
        NotifyObserversNext();

        handlerQueue = handlerQueue ?? new Queue<IAutoNode>();
        handlerStack = handlerStack ?? new Stack<IAutoNode>();
        if (IsScheduleEmpty()) {
          FillSchedule(currentNodes);
        }

        while (!IsScheduleEmpty()) {
          IAutoNode node = NextScheduledNode();
          node.HandleNode(this);
        }
      }
    }

    /// <summary>
    /// Finish traversing the current graph.
    /// </summary>
    public void EndGraph() {
      UnlockNode();
      FinishHandlingNode();

      currentGraph = null;
      currentNodes.Clear();
      ClearSchedule();

      NotifyCompleted();
    }

    /// <summary>
    /// Set the current node for the graph. Don't use this while in the middle
    /// of traversing another graph.
    /// </summary>
    public void SetCurrentNode(IAutoNode node) {
      currentNodes.Clear();
      ClearSchedule();
      currentNodes.Add(node);
      AddToSchedule(node);
    }

    public void SetCurrentNodes(List<IAutoNode> nodes) {
      currentNodes.Clear();
      currentNodes.AddRange(nodes);
    }

    public void AddNode(IAutoNode node) {
      if (!currentNodes.Contains(node)) {
        currentNodes.Add(node);
        if (!IsScheduleEmpty()) {
          AddToSchedule(node);
        }
      }
    }

    public void RemoveNode(IAutoNode node) {
      if (currentNodes.Contains(node)) {
        currentNodes.Remove(node);
      }
    }

    /// <summary>
    /// Get the current node in the graph.
    /// </summary>
    public IAutoNode GetCurrentNode() => (currentNodes != null) && (currentNodes.Count > 0) ? currentNodes[0] : null;

    /// <summary>
    /// Get the current nodes in the graph.
    /// </summary>
    public List<IAutoNode> GetCurrentNodes() => handlerQueue.Count > 0 ? new List<IAutoNode>(handlerQueue.ToArray()) : currentNodes;

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
    public bool IsFinished() => (currentGraph == null);

    /// <summary>
    /// Locks handling the graph. This will prevent the graph engine from moving
    /// onto the next node until the current node is finished.
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
    /// Unlocks handling the graph. If there was previously a lock on traversing
    /// the graph, this will release it.
    /// </summary>
    /// <returns>
    /// Whether or not the current node was actually locked.
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

    private bool IsScheduleEmpty() => DepthFirst ? (handlerStack.Count == 0) : (handlerQueue.Count == 0);

    private IAutoNode NextScheduledNode() => DepthFirst ? handlerStack.Pop() : handlerQueue.Dequeue();

    private void AddToSchedule(IAutoNode node) {
      if (DepthFirst) {
        handlerStack.Push(node);
      } else {
        handlerQueue.Enqueue(node);
      }
    }

    private void FillSchedule(List<IAutoNode> nodeList) {
      if (DepthFirst) {
        FillStack(nodeList);
      } else {
        FillQueue(nodeList);
      }
    }

    private void FillStack(List<IAutoNode> nodeList) {
      handlerStack = handlerStack ?? new Stack<IAutoNode>();
      foreach (var node in nodeList) {
        handlerStack.Push(node);
      }
    }

    private void FillQueue(List<IAutoNode> nodeList) {
      handlerQueue = handlerQueue ?? new Queue<IAutoNode>();
      foreach (var node in nodeList) {
        handlerQueue.Enqueue(node);
      }
    }

    private void ClearSchedule() {
      handlerStack.Clear();
      handlerQueue.Clear();
    }

    //-------------------------------------------------------------------------
    // IObservable API
    //-------------------------------------------------------------------------
    public IDisposable Subscribe(IObserver<GraphInfo> observer) {
      continueObservers = continueObservers ?? new List<IObserver<GraphInfo>>();
      if (!continueObservers.Contains(observer)) {
        continueObservers.Add(observer);
      }
      return new Unsubscriber<GraphInfo>(continueObservers, observer);
    }

    private void NotifyObserversNext() {
      GraphInfo info = new GraphInfo(currentNodes.Count);
      foreach (IObserver<GraphInfo> observer in continueObservers) {
        observer.OnNext(info);
      }
    }

    private void NotifyCompleted() {
      foreach (IObserver<GraphInfo> observer in continueObservers) {
        observer.OnCompleted();
      }
    }
  }
}