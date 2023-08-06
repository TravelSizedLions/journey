using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders.Tests {

  public static class TestUtils {
    
    public static T Find<T>(string name) {
      GameObject go = Find<T>(name, typeof(T));
      if (go != null) {
        return go.GetComponent<T>();
      }

      return default(T);
    }

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