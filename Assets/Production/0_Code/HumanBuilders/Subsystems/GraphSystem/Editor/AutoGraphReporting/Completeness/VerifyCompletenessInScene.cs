#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HumanBuilders.Editor {
  // Strategy Pattern Class for traversing over all scenes in the build to
  // accomplish some task.
  public static class VerifyCompletenessInScene {
    [MenuItem("Journey/Verify Graphs in Scene")]
    public static void Verify() {
      VerifyGraphs(out string message);
      if (string.IsNullOrEmpty(message)) {
        message = "All graphs in scene complete!";
      }
      Debug.Log(message);
    }
    public static bool VerifyGraphs(out string message) {
      message = "";
      foreach(AutoGraph graph in GetAutoGraphs(SceneManager.GetActiveScene())) {
        var analysis = new GraphCompletenessReport(graph);

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

          message += "Graph: " + GetHierarchyPathToGraph(graph) + "\n";
          message += " - " + totalIncompleteNodes + "/" + graph.AutoNodes.Count + " nodes incomplete:\n";
          message += submessage;
        }
      }

      return string.IsNullOrEmpty(message);
    }

    private static string GetHierarchyPathToGraph(IAutoGraph graph) {
      string path = "";
      path = graph.GraphName;

      if (typeof(AutoGraphAsset).IsAssignableFrom(graph.GetType())) {
        path = AssetDatabase.GetAssetPath((AutoGraphAsset)graph);
      } else if (typeof(AutoGraph).IsAssignableFrom(graph.GetType())) {
        Transform parent = ((AutoGraph)graph).transform.parent;
        while (parent != null) {
          path = parent.name + " > " + path;
          parent = parent.parent;
        }
      }

      return path;
    }


    public static List<AutoGraph> GetAutoGraphs(Scene scene) {
      List<AutoGraph> graphs = new List<AutoGraph>();

      foreach (GameObject obj in scene.GetRootGameObjects())  {
        graphs.AddRange(obj.GetComponentsInChildren<AutoGraph>(true));
      }

      return graphs;
    }

  }
}
#endif