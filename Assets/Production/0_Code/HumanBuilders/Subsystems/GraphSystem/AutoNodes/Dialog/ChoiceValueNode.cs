using Sirenix.OdinInspector;

namespace HumanBuilders.Graphing {
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Choice")]
  public class ChoiceValueNode : SmartAutoValueNode<IAutoNode> {
    public override IAutoNode SmartValue {
      get => null;
    }

    public VCondition Condition;


    [LabelText("Choice")]
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection ValuePort;
  }
}