
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  [CreateNodeMenu("Objective")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class ObjectiveNode : JourneyNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;


    [Space(10)]
    [PropertyOrder(999)]
    [Output(connectionType = ConnectionType.Multiple)]
    [ShowIf("Required")]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [Space(10)]
    [PropertyOrder(1)]
    public string Description;

    [Space(10)]
    [PropertyOrder(2)]
    [ShowInInspector]
    public ICondition Condition;

    [ShowInInspector]
    [PropertyOrder(3)]
    public new bool Required { 
      get => required;
      set => required = value;
    }

    [AutoTable(typeof(Reward), "Completion Rewards", NodeColors.BASIC_COLOR)]
    [PropertyOrder(4)]
    public AutoTable<Reward> Rewards;

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void PostHandle(GraphEngine graphEngine) {
      if (CanMarkCompleted()) {
        progress = QuestProgress.Completed;
        base.PostHandle(graphEngine);
      }
    }


    public bool CanMarkCompleted() {
      NodePort inPort = GetInputPort("Input");
      foreach (NodePort outputPort in inPort.GetConnections()) {
        IJourneyNode jnode = (IJourneyNode)outputPort.node;
        if (jnode.Progress != QuestProgress.Completed && jnode.Required) {
          return false;
        }
      }

      return Condition.IsMet();
    }
  }
}