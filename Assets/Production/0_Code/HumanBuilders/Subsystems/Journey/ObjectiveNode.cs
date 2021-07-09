
using Sirenix.OdinInspector;
using UnityEngine;

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
    [PropertyOrder(4)]
    [Output(connectionType = ConnectionType.Multiple)]
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



    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void PostHandle(GraphEngine graphEngine) {
      if (Condition.IsMet()) {
        progress = QuestProgress.Completed;
        base.PostHandle(graphEngine);
      }
    }
  }
}