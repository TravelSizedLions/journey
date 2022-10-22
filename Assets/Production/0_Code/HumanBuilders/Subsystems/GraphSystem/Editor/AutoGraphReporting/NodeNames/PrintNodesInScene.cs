#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HumanBuilders.Editor {
  // Strategy Pattern Class for traversing over all scenes in the build to
  // accomplish some task.
  public static class PrintNodesInScene {
    [MenuItem("Journey/Enumerate Nodes in Scene")]
    public static void PrintNodes() {
      foreach(AutoGraph graph in GetAutoGraphs(SceneManager.GetActiveScene())) {
        string message = "--- " + graph.GraphName + " ---\n";
        message += "Total Nodes: " + graph.AutoNodes.Count + "\n";
        var analyses = new AutoGraphReport<NodeNameReport>(graph).NodeAnalyses;
        analyses.ForEach((NodeNameReport n) => message += n.Name + "\n");
        message += '\n';
        Debug.Log(message);
      }
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