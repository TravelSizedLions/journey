#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace HumanBuilders.Editor {
  // Strategy Pattern Class for traversing over all scenes in the build to
  // accomplish some task.
  public static class BuildScenesTraverser {
    public static List<T> TraverseAllScenesInBuild<T>(SceneAnalyzer<T> analyzer) where T : SceneAnalysis {
      List<T> analyses = new List<T>();
      GetAllScenesInBuild().ForEach((string path) => analyses.Add(analyzer.Analyze(path)));
      return analyses;
    }

    private static List<string> GetAllScenesInBuild() {
      List<string> scenes = new List<string>();
      
      int sceneCount = SceneManager.sceneCountInBuildSettings;
      for (int i = 0; i < sceneCount; i++) {
        string path = SceneUtility.GetScenePathByBuildIndex(i);
        if (!string.IsNullOrEmpty(path)) {
          scenes.Add(path);
        }
      }

      return scenes;
    }
  }
}
#endif