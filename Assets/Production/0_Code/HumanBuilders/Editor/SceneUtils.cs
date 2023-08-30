
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSL.Editor.SceneUtilities {
  public static class SceneUtils {
    /// <summary>
    /// Gets the paths of every scene in the build.
    /// </summary>
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

    public static List<T> FindAll<T>() {
      List<T> components = new List<T>();

      List<GameObject> roots = new List<GameObject> (SceneManager.GetActiveScene().GetRootGameObjects());
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