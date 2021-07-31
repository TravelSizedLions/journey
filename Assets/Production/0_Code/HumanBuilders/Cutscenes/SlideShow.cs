using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace HumanBuilders {

  /// <summary>
  /// A class for playing a cutscene slideshow to the player.
  /// 
  /// TODO: It looks like this is disconnected from the <see cref="Slide" /> class. Should investigate if this is actually finished.
  /// </summary>
  [RequireComponent(typeof(Image))]
  public class SlideShow : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    [Header("Cutscene Images", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The list of images that make up the cutscene.
    /// </summary>
    [Tooltip("The list of images that make up the cutscene.")]
    public List<Sprite> Images;

    /// <summary>
    /// The UI component responsible for actually displaying the cutscene to the player.
    /// </summary>
    private Image screen;

    [Space(10, order=2)]

    [Header("Scene Transition Settings", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// The next scene to load after the cutscene has finished.
    /// </summary>
    [Tooltip("The next scene to load after the cutscene has finished.")]
    public SceneField Scene;

    [HideInInspector]
    [Obsolete("Use SceneField version instead")]
    public string NextScene;

    /// <summary>
    /// The next spawn point to place the player at when the cutscene has finished.
    /// </summary>
    [Tooltip("The next spawn point to place the player at when the cutscene has finished.")]
    [ValueDropdown("GetSceneSpawnPoints")]
    public string NextSpawn;

    /// <summary>
    /// The virtual camera to snap to on scene load.
    /// </summary>
    [Tooltip("The virtual camera to snap to on scene load.")]
    public string VCamName;

    /// <summary>
    /// The index of the current image being played.
    /// </summary>
    private int currentImage;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      if (Images == null) {
        Images = new List<Sprite>();
      }

      currentImage = 0;

      screen = GetComponent<Image>();

      if (Images.Count > 0) {
        screen.sprite = Images[0];
      }
    }

    private void Update() {
      if (PauseScreen.Paused) {
        return;
      }

      if (Input.GetButtonDown("Action") || Input.GetButtonDown("Jump")) {
        // if it's not the last image:
        //  Go to the next image.
        if (currentImage != Images.Count - 1) {
          NextImage();
        } else {
          ChangeScenes();
        }
      }
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Play the next image.
    /// </summary>
    public void NextImage() {
      currentImage++;
      screen.sprite = Images[currentImage];
    }

    /// <summary>
    /// Move on to the next scene.
    /// </summary>
    public void ChangeScenes() {
      TransitionManager.MakeTransition(Scene.SceneName, NextSpawn, VCamName);
    }

    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------

    #if UNITY_EDITOR
    /// <summary>
    /// Gets the list of possible spawn points in the destination scene.
    /// </summary>
    private IEnumerable<string> GetSceneSpawnPoints() => EditorUtils.GetSceneSpawnPoints(Scene);
    #endif
  }
}