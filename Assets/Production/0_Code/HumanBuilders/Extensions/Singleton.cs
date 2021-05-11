using System;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// Inherit from this base class to create a singleton.
  /// e.g. public class MyClassName : Singleton<MyClassName> {}
  /// </summary>
  public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    // Check to see if we're about to be destroyed.
    private static object lockObj = new object();
    private static T instance;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance {
      get {
        lock(lockObj) {
          if (instance == null) {
            // Search for existing instance.
            instance = (T) FindObjectOfType(typeof(T));

            // Create new instance if one doesn't already exist.
            if (instance == null) {
              // Need to create a new GameObject to attach the singleton to.
              var singletonObject = new GameObject();
              instance = singletonObject.AddComponent<T>();
              singletonObject.name = typeof(T).ToString() + " (Singleton)";

              // Make instance persistent.
              TryDontDestroyOnLoad(singletonObject);
            }
          }

          return instance;
        }
      }
    }

    protected virtual void Awake() {
      if (Instance != null && Instance != this) {
        Destroy(this);
      } else {
        transform.SetParent(null);
        TryDontDestroyOnLoad(this);
      }
    }

    private static void TryDontDestroyOnLoad(UnityEngine.Object target) {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying) {
#endif
          DontDestroyOnLoad(target);
#if UNITY_EDITOR
        }
#endif
    }

  }
}