#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders.Editor {
  public class MultiGraphsVariableUsageReport: Report {
    public bool AllComplete {get => allComplete;}
    private bool allComplete;

    public List<GraphVariableUsageReport> GraphReports { get => graphReports };
    private List<GraphVariableUsageReport> graphReports;

    public MultiGraphsVariableUsageReport(List<IAutoGraph> graphs) {
      graphReports = new List<GraphVariableUsageReport>();
      allComplete = true;
      foreach (var graph in graphs) {
        var report = new GraphVariableUsageReport(graph);
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