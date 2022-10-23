#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders.Editor {
  public class MultiGraphCompletenessReport: Report {
    public bool AllComplete {get => allComplete;}
    private bool allComplete;

    public List<GraphCompletenessReport> GraphReports;
    private List<GraphCompletenessReport> graphReports;

    public MultiGraphCompletenessReport(List<IAutoGraph> graphs) {
      graphReports = new List<GraphCompletenessReport>();
      allComplete = true;
      foreach (var graph in graphs) {
        var report = new GraphCompletenessReport(graph);
        allComplete &= report.IsComplete;
        graphReports.Add(report);
      }
    }

    protected override string BuildMessage() {
      if (allComplete) {
        return "All graphs are complete!";
      } else {
        string message = "Click to see report of incomplete graphs...\n\n";
        foreach (var report in graphReports) {
          if (!report.IsComplete) {
            message += report.Message;
          }
        }
        return message;
      }
    }
  }
}
#endif