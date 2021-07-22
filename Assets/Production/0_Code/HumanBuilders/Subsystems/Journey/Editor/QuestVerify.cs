
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders.Editor {
  public static class QuestVerify {
    [MenuItem("Journey/Analyze Quests")]
    public static void AnalyzeQuestsInProject() {
      VerifyQuestsInProject(out string message);
      Debug.Log(message);
    }

    public static bool VerifyQuestsInProject(out string message) {
      List<QuestGraph> quests = EditorUtils.FindAssetsByType<QuestGraph>();
      List<GraphReport> reports = AnalyzeQuests(quests);
      message = GraphVerify.GetReports(reports, out bool allGood);
      return allGood;
    }

    public static List<GraphReport> AnalyzeQuests(List<QuestGraph> quests) {
      List<GraphReport> reports = new List<GraphReport>(); 
      foreach (var quest in quests) {
        reports.Add(GraphVerify.AnalyzeGraph((IAutoGraph)quest));
      }

      return reports;
    }
  }
}