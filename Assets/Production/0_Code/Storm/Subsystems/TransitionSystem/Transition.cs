using System;
using System.Collections;
using System.Collections.Generic;
using Snippets;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace Storm.Subsystems.Transitions {

  /// <summary>
  /// A behavior representing a transition from one Unity scene to another.
  /// </summary>
  public class Transition : MonoBehaviour {

    /// <summary>
    /// The scene that will be loaded. This field is used for convenience in the Unity inspector.
    /// </summary>
    [SerializeField]
    [Tooltip("The scene that will be loaded.")]
    [OnValueChanged("OnSceneSelectionChanged")]
    private SceneField scene;

    /// <summary>
    /// The name of the spawn point the player will be set at.
    /// </summary>
    [SerializeField]
    [Tooltip("The name of the spawn point the player will be set at.")]
    [ValueDropdown("GetSceneSpawnPoints")]
    private string spawnPoint;

    /// <summary>
    /// The virtual camera that will be activated once the scene loads.
    /// </summary>
    [Tooltip("The virtual camera that will be activated once the scene loads.")]
    private string vCamName;

    [Space(10)]

    /// <summary>
    /// The scene that will be loaded.
    /// </summary>
    [LabelText("Destination Scene")]
    [Tooltip("The scene that will be loaded.")]
    [SerializeField]
    [ReadOnly]
    private string sceneName;
    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        if (string.IsNullOrEmpty(sceneName) && scene != null) {
          sceneName = scene.SceneName;
        }

        TransitionManager.MakeTransition(sceneName, spawnPoint, vCamName);
      }
    }
    #endregion


    #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
    private void OnSceneSelectionChanged() {
      if (!Application.isPlaying) {
        sceneName = (scene != null) ? scene.SceneName : "";
      } 
    }

    /// <summary>
    /// Gets the list of possible spawn points in the destination scene.
    /// </summary>
    private IEnumerable<string> GetSceneSpawnPoints() {
      if (!Application.isPlaying && scene != null && scene.SceneName != "") {
#if UNITY_EDITOR
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
#endif
      }
      return null;
    }
    #endregion
  }

}