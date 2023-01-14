#if UNITY_EDITOR
namespace HumanBuilders.Graphing.Editor {
  public class ProjectVariableSearchReport : ProjectReport<SceneVariableSearchReport> {
    public ProjectVariableSearchReport(string varName) : base(varName) {}
  }
}
#endif