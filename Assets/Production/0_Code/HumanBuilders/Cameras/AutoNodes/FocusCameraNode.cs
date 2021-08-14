using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.SHORT)]
  [CreateNodeMenu("Camera/Set Focus")]
  public class FocusCameraNode : AutoNode {
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
    // Fields
    //-------------------------------------------------------------------------
    [LabelText("VCam")]
    [Tooltip("The virtual camera to focus in on.")]
    public VirtualCamera VCam;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (VCam != null) {
        VCam.Activate();
      }
    }
  }
}