
namespace HumanBuilders {
  [CreateNodeMenu("Objective")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class ObjectiveNode : SojournNode {
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;


    [Output(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Output;
  }
}