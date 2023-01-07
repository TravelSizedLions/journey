#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders.Graphing.Editor {
  public class SceneGraphCompletenessReport : SceneReport {
    [MenuItem("TSL/Graphing/Analyze Graphs in Scene")]
    public static void Analyze() {
      var report = new SceneGraphCompletenessReport(SceneManager.GetActiveScene().path);
      Debug.Log(report.Message);
    }

    public bool AllComplete { get => report.AllComplete; }

    private MultiGraphCompletenessReport report;

    public SceneGraphCompletenessReport(string scenePath) : base(scenePath) {
      report = new MultiGraphCompletenessReport(GetAutoGraphs(scene));
    }

    protected override string BuildMessage() {
      return report.Message;
    }
  }
}
#endif