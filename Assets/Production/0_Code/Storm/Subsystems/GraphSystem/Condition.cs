using System;
using XNode;
using UnityEngine;

namespace Storm.Subsystems.Graph {

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
  /// This class represents a condition that can be used to wait until 
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
      Debug.Log("Output Port: " + OutputPort);
      foreach (NodePort p in xnode.Ports) {
        Debug.Log(p.fieldName);
      }
      NodePort port = xnode.GetOutputPort(OutputPort);
      NodePort nextPort = port.Connection;
      IAutoNode nextNode = (IAutoNode)nextPort.node;
      // IAutoNode nextNode = (IAutoNode)xnode.GetOutputPort(OutputPort).Connection.node;
      graphEngine.SetCurrentNode(nextNode);
      graphEngine.Continue();
    }
  }
}