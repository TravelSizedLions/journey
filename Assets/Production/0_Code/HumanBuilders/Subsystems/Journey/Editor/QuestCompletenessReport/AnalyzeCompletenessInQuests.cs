#if UNITY_EDITOR
using System.Collections.Generic;
using HumanBuilders.Graphing.Editor;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders.Editor {
  public static class AnalyzeCompletenessInQuests {

    [MenuItem("Journey/Analyze Quests in Project")]
    public static void AnalyzeQuestsInProject() {
      var graphs = EditorUtils.FindAssetsByType<QuestGraph>().ConvertAll(graph => (IAutoGraph)graph);
      var report = new MultiGraphCompletenessReport(graphs);
      if (report.AllComplete) {
        Debug.Log("All quest graphs complete!");
      } else {
        Debug.Log(report.Message);
      }
    }
  }
}

#endif