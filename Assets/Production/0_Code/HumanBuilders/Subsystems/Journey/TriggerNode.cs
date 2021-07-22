
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [CreateNodeMenu("Questing/Trigger")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class TriggerNode : AutoNode {
    [PropertyOrder(0)]
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;


    [Space(5)]
    [PropertyOrder(999)]
    [Output(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Output;


    [AutoTable(typeof(VTrigger), "Triggers", NodeColors.BASIC_COLOR)]
    public List<VTrigger> Triggers;

    public override void Handle(GraphEngine graphEngine) {
      foreach (var trigger in Triggers) {
        trigger.Pull();
      }
    }
  }
}