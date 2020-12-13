
using UnityEngine;

using Storm.Flexible.Interaction;
using Storm.Subsystems.Transitions;

using Snippets;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Storm.LevelMechanics.Doors {
  /// <summary>
  /// A door that lets the player change scenes (ala super mario brothers 2).
  /// </summary>
  public class TransitionDoor : PhysicalInteractible {
    #region Fields
    [Header("Scene Change Info", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The scene this doorway connects to.
    /// </summary>
    [Tooltip("The scene this doorway connects to.")]
    [SerializeField]
    private SceneField scene = null;

    /// <summary>
    /// The name of the spawn point the player will be placed at in the next scene.
    /// If none is specified, the player's spawn will be set to wherever the player 
    /// game object is currently located in-editor in the next scene.
    /// </summary>
    [LabelText("Spawn Point")]
    [Tooltip("The name of the spawn point the player will be placed at in the next scene.\nIf none is specified, the player's spawn will be set to wherever the player game object is currently located in-editor in the next scene.")]
    [SerializeField]
    [ValueDropdown("GetSceneSpawnPoints")]
    private string spawnName = "";
    #endregion

    #region Unity API

    protected new void Awake() {
      base.Awake();

      if (col != null) {
        Physics2D.IgnoreCollision(col, player.GetComponent<BoxCollider2D>(), true);
      }
    }
    #endregion

    #region Interactible API
    /// <summary>
    /// What the object should do when interacted with.
    /// </summary>
    public override void OnInteract() {
      if (player != null) {
        TransitionManager.MakeTransition(scene.SceneName, spawnName);
      }
    }

    /// <summary>
    /// Whether or not the indicator for this interactible should be shown.
    /// </summary>
    /// <remarks>
    /// This is used when this particular interactive object is the closest to the player. If the indicator can be shown
    /// that usually means it can be interacted with.
    /// </remarks>
    public override bool ShouldShowIndicator() {
      return true;
    }


    protected new void OnTriggerEnter2D(Collider2D other) {
      base.OnTriggerEnter2D(other);

      if (other.CompareTag("Player")) {
        if (col != null) {
          if (!Physics2D.GetIgnoreCollision(col, other)) {
            Physics2D.IgnoreCollision(col, other);
          }
        }
      }
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
          Scene openedScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

          foreach (GameObject obj in openedScene.GetRootGameObjects()) {
            foreach (var spawn in obj.GetComponentsInChildren<SpawnPoint>(true)) {
              spawnNames.Add(spawn.name);
            }
          }
          
          EditorSceneManager.CloseScene(openedScene, true);
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
