#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders {
  public static class EditorUtils {
    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object {
      List<T> assets = new List<T>();
      string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

      for (int i = 0; i < guids.Length; i++) {
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset != null) {
          assets.Add(asset);
        }
      }
      return assets;
    }


    public static List<string> GetSceneSpawnPoints(SceneField scene) {
      if (!Application.isPlaying && scene != null && scene.SceneName != "") {
        Scene selectedScene = EditorSceneManager.GetSceneByName(scene.SceneName);
        List<string> spawnNames = new List<string>();

        if (!selectedScene.IsValid()) {
          string path = AssetDatabase.GetAssetPath(scene.SceneAsset);
          if (!string.IsNullOrEmpty(path)) {
            Scene openedScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            foreach (GameObject obj in openedScene.GetRootGameObjects()) {
              foreach (var spawn in obj.GetComponentsInChildren<SpawnPoint>(true)) {
                spawnNames.Add(spawn.name);
              }
            }
            
            EditorSceneManager.CloseScene(openedScene, true);
          }
        } else {
          foreach (GameObject obj in selectedScene.GetRootGameObjects()) {
            foreach (var spawn in obj.GetComponentsInChildren<SpawnPoint>(true)) {
              spawnNames.Add(spawn.name);
            }
          }
        }

        return spawnNames;
      }
      return null;
    }
  }
}
#endif
