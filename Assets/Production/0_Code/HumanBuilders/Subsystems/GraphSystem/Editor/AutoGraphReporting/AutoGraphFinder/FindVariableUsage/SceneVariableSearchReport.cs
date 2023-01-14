#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders.Graphing.Editor {
  public class SceneVariableSearchReport : SceneReport {
    public string VariableName { get => variableName; }
    private string variableName;

    public bool ReferencesVariable { get => referencesVariable; }
    private bool referencesVariable;

    public List<AutoGraphVariableSearchReport> GraphReports { get => graphReports; }
    private List<AutoGraphVariableSearchReport> graphReports;

    public List<Tuple<string, List<string>>> References { get => references; }
    private List<Tuple<string, List<string>>> references;
    public SceneVariableSearchReport(string scenePath, params object[] extraParams) : base(scenePath) {
      variableName = (string)extraParams[0];
      graphReports = new List<AutoGraphVariableSearchReport>();
      references = new List<Tuple<string, List<string>>>();
      
      foreach (var g in GetAutoGraphs(scene)) {
        var report = new AutoGraphVariableSearchReport(g, variableName);
        graphReports.Add(report);
        if (report.ReferencesVariable) {
          referencesVariable = true;
          references.Add(Tuple.Create(g.GraphName, report.References));
        }
      }
    }
  }
}
#endif