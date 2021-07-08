
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
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      Debug.Log("ObjectiveNode");
      base.Handle(graphEngine);
    }
  }
}