#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  // Find all usages of a given scriptable variable in the project.
  public class GraphVariableUsageReport : AutoGraphReport<NodeVariableUsageReport> {
    public string VariableName { get => variableName; }
    protected string variableName;

    public GraphVariableUsageReport(IAutoGraph g, string varName) : base(g) {
      variableName = varName;
    }

    protected override string BuildMessage() {
      throw new System.NotImplementedException();
    }
  }
}

#endif