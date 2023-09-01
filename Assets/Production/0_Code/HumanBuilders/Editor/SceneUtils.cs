using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSL.Editor.SceneUtilities {
  public static class SceneUtils {

    public static List<string> GetOpenScenes() {
      List<string> scenes = new List<string>();
      for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
        scenes.Add(EditorSceneManager.GetSceneAt(i).path);
      }
      return scenes;
    }

    /// <summary>
    /// Gets the paths of every scene in the build.
    /// </summary>
    public static List<string> GetAllScenesInBuild() {
      return EditorBuildSettings.scenes.Select(sceneBuildInfo => sceneBuildInfo.path).ToList();
    }

    public static List<string> GetActiveScenesInBuild() {
      return EditorBuildSettings.scenes
        .Where(sceneBuildInfo => sceneBuildInfo.enabled)
        .Select(sceneBuildInfo => sceneBuildInfo.path)
        .ToList();
    }

    public static List<T> FindAll<T>(string name = null) {
      return FindAll<T>(SceneManager.GetActiveScene(), name);
    }

    public static List<T> FindAll<T>(Scene scene, string name = null) {
      List<T> components = new List<T>();

      scene.GetRootGameObjects().ToList().ForEach(rootObject => {
        components.AddRange(FindAllRecursive<T>(rootObject, name));
      });

      return components;
    }

    private static List<T> FindAllRecursive<T>(GameObject go, string name = null) {
      List<T> components = new List<T>();
      T comp = go.GetComponent<T>();
      if (comp != null && (name == null || (comp as Component).name == name)) {
        components.Add(comp);
      }

      for (int i = 0; i < go.transform.childCount; i++) {
        var child = go.transform.GetChild(i);
        components.AddRange(FindAllRecursive<T>(child.gameObject, name));  
      }

      return components;
    }

    /// <summary>
    /// Searches for a component of type T by its game object's name
    /// </summary>
    /// <param name="name">the name of the game object</param>
    /// <returns>T component of type T if found, default(T) otherwise.</returns>
    public static T Find<T>(string name = null) {
      return Find<T>(SceneManager.GetActiveScene(), name);
    }

    public static T Find<T>(Scene scene, string name = null) {
      foreach (var root in scene.GetRootGameObjects()) {
        T comp = SearchRecursive<T>(root, name);
        if (comp != null) {
          return comp;
        }
      }

      return default(T);
    }


    private static T SearchRecursive<T>(GameObject go, string name = null) {
      T comp = go.GetComponent<T>();
      if (comp != null && (name == null || (comp as Component).name == name)) {
        return comp;
      }

      for (int i = 0; i < go.transform.childCount; i++) {
        comp = SearchRecursive<T>(go.transform.GetChild(i).gameObject, name);
        if (comp != null) {
          return comp;
        }
      }

      return default(T);
    }

    /// <summary>
    /// Find a game object by its name.
    /// </summary>
    /// <param name="name">the name of the game object</param>
    /// <returns>The gameojbect if found, null otherwise.</returns>
    public static GameObject Find(string name) {
      return Find<Transform>(name)?.gameObject;
    }
  }
}