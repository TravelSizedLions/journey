#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  public class PrintNodeInGraphAnalyzer: AutoGraphAnalyzer<NodeNameAnalysis> {
    public override NodeNameAnalysis Analyze(IAutoNode node) {
      return new NodeNameAnalysis(node.NodeName);
    }
  }
}
#endif