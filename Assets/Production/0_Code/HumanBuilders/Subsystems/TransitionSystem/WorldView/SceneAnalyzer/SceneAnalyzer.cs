using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace TSL.Subsystems.WorldView {
  public static class SceneAnalyzer {
    public static SceneData Analyze(Scene scene) {
      SceneData data = new SceneData();
      data.Construct(scene);
      return data;
    }

    public static bool TryUpdate(Scene scene, ref SceneData data) {
      SceneData updated = new SceneData();
      updated.Construct(scene);
      if (data == updated) {
        return false;
      }

      data = updated;
      return true;
    }
  }
}