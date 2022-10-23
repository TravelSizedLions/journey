#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders.Editor {
  public class SceneReport : Report {

    public string ScenePath { get => path; }
    protected string path;

    public Scene Scene { get => scene; }
    protected Scene scene;

    public SceneReport(string scenePath) {
      path = scenePath;
      scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
    }

    protected override string BuildMessage() {
      return path;
    }

    public static List<IAutoGraph> GetAutoGraphs(Scene scene) {
      List<IAutoGraph> graphs = new List<IAutoGraph>();

      foreach (GameObject obj in scene.GetRootGameObjects()) {
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