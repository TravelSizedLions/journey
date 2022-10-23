#if UNITY_EDITOR
using System.Collections.Generic;

namespace HumanBuilders.Editor {
  // Find all usages of a given scriptable variable in the project.
  public class ProjectVariableUsageReport : ProjectReport<SceneVariableUsageReport> {
    public string VariableName { get => variableName; }
    protected string variableName;

    public ProjectVariableUsageReport(string varName) {
      variableName = varName;
      sceneReports = new List<SceneVariableUsageReport>();
      GetAllScenesInBuild().ForEach(
        (string path) => {
          sceneReports.Add(new SceneVariableUsageReport(path, varName));
        }
      );
    }

    protected override string BuildMessage() {
      return base.BuildMessage();
    }
  }
}

#endif