#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  public class NodeNameReport : AutoNodeReport {
    public readonly string Name;
    public NodeNameReport(string name) {
      Name = name;
    }
  }
}
#endif