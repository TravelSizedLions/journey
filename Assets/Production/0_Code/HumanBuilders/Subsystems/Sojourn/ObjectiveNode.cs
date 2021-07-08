
using UnityEngine;

namespace HumanBuilders {
  [CreateNodeMenu("Objective")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class ObjectiveNode : AutoNode, ISojournNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Output(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    public ICondition Condition;

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void PostHandle(GraphEngine graphEngine) {
      if (Condition.IsMet()) {
        base.PostHandle(graphEngine);
      }
    }
  }
}