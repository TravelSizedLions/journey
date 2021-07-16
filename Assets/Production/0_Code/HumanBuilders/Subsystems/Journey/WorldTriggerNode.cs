
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
    [AutoTable(typeof(VSetter), "World Change Triggers", NodeColors.BASIC_COLOR)]
    public AutoTable<VSetter> Triggers;


    public override void Handle(GraphEngine graphEngine) {
      QuestGraph quest = (QuestGraph)graph;
      if (CanMarkCompleted()) {
        progress = QuestProgress.Completed;
        foreach (var setter in Triggers) {
          setter.Set();
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
        if (jnode.Progress != QuestProgress.Completed && jnode.Required) {
          return false;
        }
      }

      return progress == QuestProgress.Unavailable;
    }
  }
}