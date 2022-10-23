#if UNITY_EDITOR

using UnityEditor.SceneManagement;

namespace HumanBuilders.Editor {
  // Find all usages of a given scriptable variable in the project.
  public class SceneVariableUsageReport : SceneReport {
    public string VariableName { get => variableName; }
    protected string variableName;

    public SceneVariableUsageReport(string scenePath, string varName) : base(scenePath) {
      variableName = varName;
      var graphsReport = new MultiGraphCompletenessReport(GetAutoGraphs(scene));
    }

    protected override string BuildMessage() {
      throw new System.NotImplementedException();
    }
  }
}

#endif