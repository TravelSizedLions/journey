using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [CreateNodeMenu("Questing/Quest Trigger")]
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

    [Space(5)]
    [AutoTable(typeof(VTrigger), "Triggers", NodeColors.BASIC_COLOR)]
    public List<VTrigger> Triggers;

    [Space(5)]
    [LabelWidth(140)]
    public bool CheckQuestProgress = true;

    public override void Handle(GraphEngine graphEngine) {
      foreach (var trigger in Triggers) {
        trigger.Pull();
      }
    }

    public override void PostHandle(GraphEngine graphEngine) {
      base.PostHandle(graphEngine);
      if (CheckQuestProgress) {
        Journey.Step();
      }
    }
  }
}