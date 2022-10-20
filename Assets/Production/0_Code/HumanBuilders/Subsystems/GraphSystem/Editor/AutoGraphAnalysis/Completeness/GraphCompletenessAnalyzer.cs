#if UNITY_EDITOR

using XNode;

namespace HumanBuilders.Editor {
  public class GraphCompletenessAnalyzer: AutoGraphAnalyzer<NodeCompletenessAnalysis> {
    public override NodeCompletenessAnalysis Analyze(IAutoNode node) {
      var report = new NodeCompletenessAnalysis();
      report.Node = node;
      report.TotalPorts = 0;
      report.TotalInputPorts = 0;
      report.TotalOutputPorts = 0;

      foreach (NodePort port in node.Ports) {
        report.TotalPorts++;
        report.TotalOutputPorts += port.IsOutput ? 1 : 0;
        report.TotalInputPorts += port.IsInput ? 1 : 0;
      }

      report.Complete = node.IsNodeComplete();
      report.TotalUnconnectedPorts = report.Complete ? 0 : node.TotalDisconnectedPorts();
      return report;
    }
  }
}
#endif