#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HumanBuilders.Editor {
  // Strategy Pattern Class for traversing over all scenes in the build to
  // accomplish some task.
  public static class SceneGraphUtils {
    [MenuItem("Journey/Analyze Graphs in Scene")]
    public static void Analyze() {
      var report = new MultiGraphCompletenessReport(GetAutoGraphs(SceneManager.GetActiveScene()));
      Debug.Log(report.Message);
    }

    public static List<IAutoGraph> GetAutoGraphs(Scene scene) {
      List<IAutoGraph> graphs = new List<IAutoGraph>();

      foreach (GameObject obj in scene.GetRootGameObjects())  {
        var graphsInObject = obj.GetComponentsInChildren<AutoGraph>(true);
        foreach (var graph in graphsInObject) {
          if (graph != null) {
            graphs.Add(graph);
          }
        }
      }

      return graphs;
    }

  }
}
#endif