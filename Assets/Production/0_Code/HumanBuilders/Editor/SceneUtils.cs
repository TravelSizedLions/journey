
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace TSL.Editor.SceneUtilities {
  public static class SceneUtils {
    public static List<string> GetAllScenesInBuild() {
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