using Sirenix.OdinInspector;
namespace HumanBuilders {

  public abstract class AutoValueNode : AutoNode {
    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Output;

    public abstract object Value { get; }
  }
}