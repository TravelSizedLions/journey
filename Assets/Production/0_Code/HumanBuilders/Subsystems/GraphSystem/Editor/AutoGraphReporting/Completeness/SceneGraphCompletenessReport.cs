#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders.Editor {
  public class SceneGraphCompletenessReport : SceneReport {
    [MenuItem("Journey/Analyze Graphs in Scene")]
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