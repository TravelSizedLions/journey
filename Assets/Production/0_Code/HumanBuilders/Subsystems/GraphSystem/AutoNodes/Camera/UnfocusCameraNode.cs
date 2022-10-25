using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders.Graphing {
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.SHORT)]
  [CreateNodeMenu("Camera/Clear Focus")]
  public class UnfocusCameraNode : AutoNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8)]
    [PropertyOrder(999)]
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      TargettingCamera.ClearTarget();
    }
  }
}