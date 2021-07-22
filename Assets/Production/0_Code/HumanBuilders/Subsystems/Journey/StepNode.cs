
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [CreateNodeMenu("Questing/Quest Step")]
  [NodeWidth(150)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class StepNode : AutoNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;


    [Space(5)]
    [PropertyOrder(999)]
    [Output(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Output;

    public override void Handle(GraphEngine graphEngine) {
      Journey.Step();
    }
  }
}