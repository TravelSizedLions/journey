#if UNITY_EDITOR

namespace HumanBuilders.Graphing.Editor {
  public class GraphCompletenessReport: AutoGraphReport<NodeCompletenessReport>  {

    public bool IsComplete { get => isComplete; }
    private bool isComplete;

    public int TotalIncomplete { get => totalIncomplete; }
    private int totalIncomplete;

    public GraphCompletenessReport(IAutoGraph g) : base(g) {
      totalIncomplete = 0;
      foreach (var nodeReport in NodeReports) {
        totalIncomplete += nodeReport.Complete ? 0 : 1;
      }
      isComplete = totalIncomplete == 0;
    }

    protected override string BuildMessage() {
      string message = "";
      if (!isComplete) {
        string pathInSceneHierarchy = GetHierarchyPathToGraph(graph);
        message += string.Format("Graph: {0}\n", pathInSceneHierarchy);
        message += string.Format(" - {0}/{1} nodes incomplete:\n", totalIncomplete, graph.AutoNodes.Count);
        foreach (var report in nodeReports) {
          if (report.HasMessage) {
            message += string.Format("   - {0}\n", report.Message);
          }
        }
      }

      return message;
    }
  }
}
#endif