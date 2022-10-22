#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders.Editor {
  public static class AnalyzeCompletenessInQuests {

    [MenuItem("Journey/Analyze Quests in Project")]
    public static void AnalyzeQuestsInProject() {
      if (!AllQuestsComplete(out string message)) {
        Debug.Log(message);
      } else {
        Debug.Log("All quest graphs complete!");
      }
    }

    public static bool AllQuestsComplete(out string message) {
      message = "";
      List<QuestGraph> quests = EditorUtils.FindAssetsByType<QuestGraph>();

      foreach (var quest in quests) {
        var analysis = new GraphCompletenessReport(quest);
        
        if (!analysis.IsComplete) {
          if (message == "") {
            message += "Click to see report of incomplete graphs...\n\n";
          }

          int totalIncompleteNodes = 0;
          string submessage = "";
          analysis.NodeAnalyses.ForEach((NodeCompletenessReport a) => {
            totalIncompleteNodes += a.Complete ? 0 : 1;
            if (!a.Complete) {
              submessage += "   - " + a.Node.GetType().FullName.Split('.')[a.Node.GetType().FullName.Split('.').Length-1] + ": ";
              submessage += a.TotalUnconnectedPorts + " unconnected ports\n";
            }
          });

          message += "Graph: " + quest.Title + "\n";
          message += " - " + AssetDatabase.GetAssetPath(quest) + "\n";
          message += " - " + totalIncompleteNodes + "/" + quest.AutoNodes.Count + " nodes incomplete:\n";
          message += submessage;
        }
      }

      return string.IsNullOrEmpty(message);
    }
  }
}

#endif