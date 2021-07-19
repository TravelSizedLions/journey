
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  [CreateNodeMenu("World Trigger")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class WorldTriggerNode : JourneyNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;


    [Space(5)]
    [PropertyOrder(999)]
    [Output(connectionType = ConnectionType.Multiple)]
    [ShowIf("Required")]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(VTrigger), "World Change Triggers", NodeColors.BASIC_COLOR)]
    public List<VTrigger> Triggers;


    public override void Handle(GraphEngine graphEngine) {
      QuestGraph quest = (QuestGraph)graph;
      if (CanMarkCompleted()) {
        progress = QuestProgress.Completed;
        foreach (var trigger in Triggers) {
          trigger.Pull();
        }
      }
    }

    public override void PostHandle(GraphEngine graphEngine) {
      if (progress == QuestProgress.Completed) {
        base.PostHandle(graphEngine);
      }
    }

    private bool CanMarkCompleted() {
      NodePort inPort = GetInputPort("Input");
      foreach (NodePort outputPort in inPort.GetConnections()) {
        IJourneyNode jnode = (IJourneyNode)outputPort.node;
        if (jnode.Progress != QuestProgress.Completed) {
          return false;
        }
      }

      return progress == QuestProgress.Unavailable;
    }
  }
}