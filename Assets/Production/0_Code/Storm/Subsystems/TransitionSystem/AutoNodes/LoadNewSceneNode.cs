using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace HumanBuilders {

  /// <summary>
  /// A node that ends a graph by loading a cutscene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("Scenes/Load New Scene")]
  public class LoadNewSceneNode : AutoNode {
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous nodes(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The scene that will be loaded. This field is used for convenience in the Unity inspector.
    /// </summary>
    [SerializeField]
    [Tooltip("The scene that will be loaded.")]
    private SceneField scene = null;

    /// <summary>
    /// The name of the spawn point the player will be set at.
    /// </summary>
    [SerializeField]
    [Tooltip("The name of the spawn point the player will be set at.")]
    [ValueDropdown("GetSceneSpawnPoints")]
    private string spawnPoint = "";
    #endregion

    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      TransitionManager.MakeTransition(scene.SceneName, spawnPoint);
    }

    public override void PostHandle(GraphEngine graphEngine) {
      graphEngine.EndGraph();
    }
    #endregion
  
      #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------

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