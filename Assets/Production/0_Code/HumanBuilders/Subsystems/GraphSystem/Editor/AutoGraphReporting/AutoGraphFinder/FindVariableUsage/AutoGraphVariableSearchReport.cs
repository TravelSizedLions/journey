#if UNITY_EDITOR
using System.Collections.Generic;

namespace HumanBuilders.Graphing.Editor {

  public class AutoGraphVariableSearchReport : AutoGraphReport<AutoNodeVariableSearchReport> {
    
    public string VariableName { get => variableName; }
    private string variableName;
    
    public bool ReferencesVariable { get => referencesVariable; }
    private bool referencesVariable;

    public List<string> References { get => references; }
    private List<string> references;

    public AutoGraphVariableSearchReport(IAutoGraph g, string varName): base(g, varName) {      
      variableName = varName;
      references = new List<string>();
      foreach (var r in nodeReports) {
        if (r.ReferencesVariable) {
          referencesVariable = true;
          references.Add(r.Message);
        }
      }
    }
    // Start is called before the first frame update
    protected override string BuildMessage() {
      if (referencesVariable) {
        string path = GetHierarchyPathToGraph(graph);
        string message = string.Format("{0}\n\n", path);

        foreach (var r in references) {
          message += string.Format(" - {0}\n", r);
        }

        return message;
      }

      return "";
    }
  }
}
#endif