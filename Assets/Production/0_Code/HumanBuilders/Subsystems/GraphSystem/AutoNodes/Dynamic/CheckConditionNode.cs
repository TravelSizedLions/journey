using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders.Graphing {
  /// <summary>
  /// Checks a condition.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Conditions/Single AutoCondition")]
  public class CheckConditionNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [GUIColor(1f, 1f, 1f)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The condition to check.
    /// </summary>
    public AutoCondition Condition;

    [Space(8, order=1)]

    /// <summary>
    /// The output connection if the condition is met.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Pass;

    /// <summary>
    /// The output connection if the condition fails.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Fail;

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override IAutoNode GetNextNode() {
      string portName = (Condition != null && Condition.IsMet()) ? "Pass" : "Fail";
      NodePort outputPort = GetOutputPort(portName);
      NodePort inputPort = outputPort.Connection;
      return (IAutoNode)inputPort.node;
    }
  }
}