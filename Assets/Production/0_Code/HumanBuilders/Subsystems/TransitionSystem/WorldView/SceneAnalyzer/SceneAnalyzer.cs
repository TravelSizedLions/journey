using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace TSL.Subsystems.WorldView {
  public static class SceneAnalyzer {
    public static SceneData Analyze(Scene scene) {
      return new SceneData(scene);
    }

    public static bool TryUpdate(Scene scene, ref SceneData data) {
      SceneData updated = new SceneData(scene);
      if (data == updated) {
        return false;
      }

      data = updated;
      return true;
    }
  }
}