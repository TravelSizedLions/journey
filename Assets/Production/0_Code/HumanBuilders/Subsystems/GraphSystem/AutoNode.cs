using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using XNode;

namespace HumanBuilders {

  /// <summary>
  /// The base class for Graph Nodes. Follows the "Template Method" pattern.
  /// </summary>
  /// <remarks>
  /// Template Method Pattern: https://sourcemaking.com/design_patterns/template_method
  /// </remarks>
  public abstract class AutoNode : Node, IAutoNode {

    //---------------------------------------------------------------------
    // Fields
    //---------------------------------------------------------------------

    /// <summary>
    /// The list of conditions to check. The conditions will be checked each
    /// frame in the order listed. If any is met, the transition that
    /// corresponds to the condition will be used.
    /// </summary>
    /// <remarks>
    /// To keep things looking clean in the inspector, this variable is kept
    /// private for the following reasons:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     Some nodes already have predefined transitions.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     Not all nodes need the flexibility of being able to dynamically
    /// define how they transition to different nodes.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     Not all nodes SHOULD ALLOW for different ways to transition to
    ///     different nodes.
    ///     </description>
    ///   </item>
    /// </list>
    /// 
    /// To create a node that implements dynamic conditions:
    /// <list type="number">
    ///   <item>
    ///     <description>
    ///     Sub-class from <see cref="AutoNode" />.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     Add a public facing list of conditions to your node so you can add
    ///     conditions in the inspector.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     In the Awake() or Start() method for your node, make the call to 
    ///     <see cref="IAutoNode.RegisterConditions" />.
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
    private List<Condition> registeredConditions = null;

    /// <summary>
    /// A reference to the Player.
    /// </summary>
    protected static PlayerCharacter player;

    /// <summary>
    /// A reference to the graph traversal engine that last handled this node. 
    /// </summary>
    private GraphEngine engine;

    //---------------------------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------------------------
    /// <summary>
    /// Handle this node.
    /// </summary>
    /// <param name="graphEngine">
    /// The graph engine that called this into this node.
    /// </param>
    /// <remarks>
    /// This is a template method (see
    /// https://sourcemaking.com/design_patterns/template_method). Sub-class
    /// from this class and override Handle() and PostHandle() to create your
    /// own custom behavior.
    /// </remarks>
    public void HandleNode(GraphEngine graphEngine) {
      if (player == null) {
        player = GameManager.Player;
      }

      if (graphEngine.StartHandlingNode()) {

        // Hook method. Implement this in a sub-class.
        engine = graphEngine;
        Handle(graphEngine);

        /** 
         * Some nodes spin off coroutines in their Handle() method. 
         * 
         * When this is the case, it's possible for the node to lock "finishing" the node
         * until the coroutine is done. In this case, we spin up a second
         * coroutine to wait until the node truly is finished.
         */
        if (graphEngine.FinishHandlingNode()) {
          // Hook method. Implement this in a sub-class.
          PostHandle(graphEngine);
        } else {
          graphEngine.StartThread(_WaitUntilFinished(graphEngine));
        }
      }
    }

    /// <summary>
    /// How to handle this node.
    /// </summary>
    /// <param name="graphEngine">The graph traversal engine that called this node</param>
    /// <remarks>
    /// This is a hook method. Override this in a sub-class of <see cref="AutoNode"/> in
    /// order to write the actual behavior of the node. 
    /// </remarks>
    public virtual void Handle(GraphEngine graphEngine) {

    }

    /// <summary>
    /// What to do after handling this node.
    /// </summary>
    /// <param name="graphEngine">
    /// The graph engine that called this into this node.
    /// </param>
    /// <remarks>
    /// Usually, this will either be "go to the next node in the graph" or 
    /// "do nothing (and wait for the next player input)." The default behavior
    /// handles the first case.
    /// <para/>
    /// This is a hook method. Override this in a sub-class of <see cref="AutoNode"/> in
    /// order to write the actual desired behavior of the node. 
    /// </remarks>
    public virtual void PostHandle(GraphEngine graphEngine) {
      IAutoNode node = GetNextNode();
      graphEngine.SetCurrentNode(node);
      graphEngine.Continue();
    }

    /// <summary>
    /// Get the next node in the dialog graph.
    /// </summary>
    /// <returns>The next node in the dialog graph.</returns>
    public virtual IAutoNode GetNextNode() {
      NodePort port = GetOutputPort("Output").Connection;
      if (port != null) {
        return (IAutoNode) port.node;
      } else {
        Debug.LogError("Please connect output port of node: " + name);
        return null;
      }
    }

    /// <summary>
    /// Waits to call the PostHandle hook until after the node has actually
    /// finished handling itself. This coroutine spin up if the
    /// GraphEngine locks on the current node. (See <see cref="GraphEngine.LockNode"/>)
    /// </summary>
    private IEnumerator _WaitUntilFinished(GraphEngine graphEngine) {
      while (!graphEngine.FinishHandlingNode()) {
        yield return null;
      }

      PostHandle(graphEngine);
    }

    /// <summary>
    /// Add a list of conditional transitions to this node. These conditions
    /// will be checked each frame.
    /// </summary>
    /// <param name="conditions">The list of conditions to register.</param>
    /// <param name="outputPort">The name of the output port these conditions
    /// map to.</param>
    public void RegisterConditions(List<Condition> conditions, string outputPort) {
      if (registeredConditions == null) {
        registeredConditions = new List<Condition>();
      }

      for (int i = 0; i < conditions.Count; i++) {
        conditions[i].OutputPort = outputPort + " " + i;
      }

      registeredConditions.AddRange(conditions);
    }

    /// <summary>
    /// Check any transition conditions registered on this node. 
    /// </summary>
    /// <returns>True if any condition was met. False otherwise.</returns>
    public bool CheckConditions() {
      if (registeredConditions != null) {
        // If there are any registered conditions, check them to see if the node
        // should transition.
        foreach (Condition c in registeredConditions) {
          if (c.IsMet()) {
            c.Transition(engine, this);
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Whether or not the node is considered fully set up. Usually
    /// this would include things like all node ports being linked,
    /// or all required properties filled out.
    /// </summary>
    public virtual bool IsComplete() {
      foreach (NodePort port in Ports) {
        if (!port.IsConnected || port.GetConnections().Count == 0) {
          return false;
        }
      }

      return true;
    }

    public virtual int TotalDisconnectedPorts() {
      int disconnected = 0;
      foreach (NodePort port in Ports) {
        if (!port.IsConnected || port.GetConnections().Count == 0) {
          disconnected++;
        }
      }

      return disconnected;
    }

    /// <summary>
    /// Connect this node to another node.
    /// </summary>
    /// <param name="dstNode">The node to connect to.</param>
    /// <param name="srcPort">The name of the port on this node to create a
    /// connection from. </param>
    /// <param name="dstPort">The name of the port on the destination node to
    /// create a connection to.</param>
    /// <param name="direction">Whether the connection is going from output
    /// (source) to input (dest), or from input (source) to output (dest).</param>
    public void ConnectTo(AutoNode dstNode, string srcPort="Output", string dstPort="Input", ConnectionDirection direction = ConnectionDirection.Forward) {
      NodePort outPort;
      NodePort inPort;

      if (direction == ConnectionDirection.Forward) {
        outPort = GetOutputPort(srcPort);
        inPort = dstNode.GetInputPort(dstPort);
      } else {
        outPort = dstNode.GetOutputPort(dstPort);
        inPort = GetInputPort(srcPort);
      }

      outPort.Connect(inPort);
    }

    //---------------------------------------------------------------------
    // XNode API
    //---------------------------------------------------------------------
    /// <summary> 
    /// Returns a value based on requested port output.
    /// </summary>
    public override object GetValue(NodePort port) {
      // Don't remove this. Prevents an annoying warning from the XNode library.
      return null;
    }
  }
}