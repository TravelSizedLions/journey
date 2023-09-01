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

    public static List<T> FindAll<T>() {
      return FindAll<T>(SceneManager.GetActiveScene());
    }

    public static List<T> FindAll<T>(Scene scene) {
      List<T> components = new List<T>();

      List<GameObject> roots = new List<GameObject>(scene.GetRootGameObjects());
      roots.ForEach(rootObject => {
        foreach (Transform child in rootObject.GetComponentInChildren<Transform>(true)) {
          T comp = child.GetComponent<T>();
          if (comp != null) {
            components.Add(comp);
          }
        }
      });

      return components;
    }

    /// <summary>
    /// Searches for a component of type T by its game object's name
    /// </summary>
    /// <param name="name">the name of the game object</param>
    /// <typeparam name="T">the type of the component</typeparam>
    /// <returns>T component of type T if found, null otherwise.</returns>
    public static T Find<T>(string name) {
      GameObject go = Find<T>(name, typeof(T));
      if (go != null) {
        return go.GetComponent<T>();
      }

      return default(T);
    }

    /// <summary>
    /// Find a game object by its name.
    /// </summary>
    /// <param name="name">the name of the game object</param>
    /// <returns>The gameojbect if found, null otherwise.</returns>
    public static GameObject Find(string name) {
      return Find<Transform>(name, null);
    }

    public static GameObject Find<T>(string name, Type t) {
      GameObject found = SearchRoots<T>(SceneManager.GetActiveScene().GetRootGameObjects(), name, t);
      if (found != null) {
        return found;
      }

      GameObject go = new GameObject();
      UnityEngine.GameObject.DontDestroyOnLoad(go);

      found = SearchRoots<T>(go.scene.GetRootGameObjects(), name, t);
      if (found != null) {
        return found;
      }

      return null;
    }

    public static GameObject SearchRoots<T>(GameObject[] roots, string name, Type t) {
      foreach (GameObject root in roots) {
        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children) {
          if (child.name == name && (t == null || child.GetComponent<T>() != null)) {
            return child.gameObject;
          }
        }
      }
      return null;
    }
  }
}