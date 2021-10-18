namespace HumanBuilders {
  public abstract class SmartAutoValueNode<T>: AutoValueNode {
    public override object Value { get => SmartValue; }
    public abstract T SmartValue { get; }
  }
}