#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  public class NodeNameAnalysis : AutoGraphAnalysis {
    public readonly string Name;
    public NodeNameAnalysis(string name) {
      Name = name;
    }
  }
}
#endif