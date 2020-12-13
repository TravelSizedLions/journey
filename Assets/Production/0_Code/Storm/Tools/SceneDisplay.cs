using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Storm.Subsystems.Transitions;

namespace Storm.Tools {
  /// <summary>
  /// This is a utility behavior that displays the current name of the scene in game.
  /// </summary>
  public class SceneDisplay : MonoBehaviour {
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The text UI element used to display the scene's name.
    /// </summary>
    [SerializeField]
    [Tooltip("The text UI element used to display the scene's name.")]
    private TextMeshProUGUI text = null;

    /// <summary>
    /// The toggle button for this script.
    /// </summary>
    [SerializeField]
    [Tooltip("The toggle button for this script.")]
    private Toggle toggle = null;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      if (toggle != null) {
        if (!toggle.isOn) {
          enabled = false;
        }
      }
    }

    private void OnEnable() {
      SceneManager.sceneUnloaded += OnSceneUnloaded;
      DisplaySceneName();
    }

    private void OnDisable() {
      HideSceneName();
      SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    #endregion


    public void Toggle() {
      enabled = !enabled;
    }

    /// <summary>
    /// Callback for scene loading.
    /// </summary>
    /// <param name="scene">The scene that's been loaded.</param>
    /// <param name="mode">The loading mode for the scene (either Single or Additive).</param>
    private void OnSceneUnloaded(Scene scene) {
      Debug.Log("Display Name Change!");
      DisplaySceneName();
    }

    /// <summary>
    /// Display the name of the current scene.
    /// </summary>
    private void DisplaySceneName() {
      if (text != null) {
        text.text = SceneManager.GetActiveScene().name;
      }
    }

    /// <summary>
    /// Hide the name of the current scene.
    /// </summary>
    private void HideSceneName() {
      if (text != null) {
        text.text = "";
      }
    }
  }

}