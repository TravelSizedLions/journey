using System;
using XNode;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// Interface for a dynamic condition. Nodes that support dynamic conditions
  /// will have a list of conditions checked every frame. A condition that's met
  /// causes the graph to transition to another node.
  /// </summary>
  public interface ICondition {
    /// <summary>
    /// The name of the output port this condition maps to.
    /// </summary>
    /// <seealso cref="Condition.OutputPort" />
    string OutputPort { get; set; }

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Check the given condition.
    /// </summary>
    /// <seealso cref="Condition.ConditionMet" />
    bool ConditionMet();

    /// <summary>
    /// Perform the transition from one node to the next.
    /// </summary>
    /// <param name="graphEngine">The graph traversal engine making the transition</param>
    /// <param name="node">The node to transition from.</param>
    /// <param node="index">The index of the condition from the list</param>
    /// <seealso cref="Condition.Transition" />
    void Transition(GraphEngine graphEngine, IAutoNode node);

    #endregion
  }

  /// <summary>
  /// This class represents a dynamic condition that can be used to transition
  /// to another node. Nodes that support dynamic conditions will check the list
  /// of added conditions every frame until one is met.
  /// </summary>
  public class Condition : MonoBehaviour, ICondition {
    /// <summary>
    /// The name of the output port this condition maps to.
    /// </summary>
    public string OutputPort { get; set; }

    /// <summary>
    /// Check the given condition.
    /// </summary>
    public virtual bool ConditionMet() => false;

    /// <summary>
    /// Perform the transition from one node to the next.
    /// </summary>
    /// <param name="graphEngine">The graph traversal engine making the transition</param>
    /// <param name="node">The node to transition from.</param>
    public virtual void Transition(GraphEngine graphEngine, IAutoNode node) {
      Node xnode = (Node)node;

      NodePort port = xnode.GetOutputPort(OutputPort);
      NodePort nextPort = port.Connection;
      IAutoNode nextNode = (IAutoNode)nextPort.node;

      graphEngine.SetCurrentNode(nextNode);
      graphEngine.Continue();
    }
  }
}