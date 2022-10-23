#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  // Find all usages of a given scriptable variable in the project.
  public class NodeVariableUsageReport : AutoNodeReport {
    public string VariableName { get => VariableName; }
    protected string variableName;

    public NodeVariableUsageReport(IAutoNode n, string varName) : base(n) {
      variableName = varName;
    }

    protected override string BuildMessage() {
      throw new System.NotImplementedException();
    }
  }
}

#endif