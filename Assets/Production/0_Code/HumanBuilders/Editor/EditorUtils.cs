#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
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

    /// <summary>
    /// Within a code assembly, searches for all subtypes of the given type.
    /// </summary>
    /// <param name="assemblyName">The name of the C# assembly.</param>
    /// <param name="t">The type to search for.</param>
    /// <returns>The list of types in the assebly that are a subtype of t.</returns>
    public static List<Type> GetSubtypesOfTypeInAssembly(string assemblyName, Type t) {
      List<Type> results = new List<Type>();

      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        if (assembly.FullName.StartsWith(assemblyName)) {
          foreach (Type type in assembly.GetTypes()) {
            if (type.IsSubclassOf(t))
              results.Add(type);
          }
          break;
        }
      }

      return results;
    }
  }
}

#endif
