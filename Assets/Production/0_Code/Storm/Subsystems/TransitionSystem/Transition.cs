using System;
using System.Collections;
using System.Collections.Generic;
using Snippets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Storm.Subsystems.Transitions {

  /// <summary>
  /// A behavior representing a transition from one Unity scene to another.
  /// </summary>
  public class Transition : MonoBehaviour {


    /// <summary>
    /// The Scene that will be loaded.
    /// </summary>
    [Tooltip("The scene that will be loaded.")]
    public SceneField scene;

    /// <summary>
    /// The spawn point the player will be set at.
    /// </summary>
    [Tooltip("The spawn point the player will be set at.")]
    public GuidReference spawnPoint;

    /// <summary>
    /// The virtual camera that will be activated once the scene loads.
    /// </summary>
    [Tooltip("The virtual camera that will be activated once the scene loads.")]
    public string vcamName;

    [Space(10)]
    /// <summary>
    /// The name of the spawn point the player will be set at.
    /// </summary>
    [LabelText("Deprecated - Spawn Point Name")]
    [Tooltip("(Deprecated) - The name of the spawn point the player will be set at.")]
    [Obsolete("Use Transition.spawnPoint instead.")]
    public string spawnPointName;

    /// <summary>
    /// The scene that will be loaded.
    /// </summary>
    [LabelText("Deprecated - Dest. Scene")]
    [Obsolete("Use the Transition.scene instead.")]
    [Tooltip("(Deprecated) - The scene that will be loaded.")]
    public string destinationScene;

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        string sceneName = (scene != null) ? scene.SceneName : destinationScene;
        if (spawnPoint != null) {
          TransitionManager.MakeTransition(sceneName, spawnPoint, vcamName);
        } else {
          TransitionManager.MakeTransition(sceneName, spawnPointName, vcamName);
        }
      }
    }
    #endregion
  }

}