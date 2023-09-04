#if UNITY_EDITOR
using HumanBuilders;
using TSL.Editor.SceneUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSL.Subsystems.WorldView {
  [InitializeOnLoad]
  internal class WorldViewSynchronizer {
    public static void Disable() {
      EditorSceneManager.sceneSaved -= OnSceneSaved;
    }

    public static void Enable() {
      EditorSceneManager.sceneSaved += OnSceneSaved;
    }

    private WorldViewSynchronizer() {
      Enable();
    }

    public static WorldViewSynchronizer Instance => instance;
    private static WorldViewSynchronizer instance = new WorldViewSynchronizer();

    private static void OnSceneSaved(Scene scene) {
      WorldViewGraph graph = AssetDatabase.LoadAssetAtPath<WorldViewGraph>(WorldViewSettings.GRAPH_PATH);
      SceneNode node = graph[scene.name];
      if (node == null) {;
        Debug.LogWarning($"Could not find scene {scene.name}. This may be because the scene is not enabled in the build.");
        return;
      }

      Debug.Log($"Syncing transitions on node {node.name}");

      if (node.Sync(scene)) {
        AssetDatabase.SaveAssets();
      }
    }
  }
}
#endif