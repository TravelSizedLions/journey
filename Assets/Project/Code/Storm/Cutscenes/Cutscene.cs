using System.Collections;
using System.Collections.Generic;
using Storm.TransitionSystem;
using UnityEngine;
using UnityEngine.UI;


namespace Storm.Cutscenes {

  /// <summary>
  /// A class for playing a cutscene slideshow to the player.
  /// 
  /// TODO: It looks like this is disconnected from the <see cref="Slide" /> class. Should investigate if this is actually finished.
  /// </summary>
  [RequireComponent(typeof(Image))]
  public class Cutscene : MonoBehaviour {

    #region Variables
    #region Cutscene Images
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
    #endregion

    #region Scene Transition Settings
    [Header("Scene Transition Settings", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// The next scene to load after the cutscene has finished.
    /// </summary>
    [Tooltip("The next scene to load after the cutscene has finished.")]
    public string NextScene;


    /// <summary>
    /// The next spawn point to place the player at when the cutscene has finished.
    /// </summary>
    [Tooltip("The next spawn point to place the player at when the cutscene has finished.")]
    public string NextSpawn;

    /// <summary>
    /// The index of the current image being played.
    /// </summary>
    private int currentImage;

    #endregion
    #endregion

    #region Unity API
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
      if (Input.GetKeyDown(KeyCode.Space)) {
        // if it's not the last image:
        //  Go to the next image.
        if (currentImage != Images.Count - 1) {
          NextImage();
        } else {
          ChangeScenes();
        }
      }
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    public void NextImage() {
      currentImage++;
      screen.sprite = Images[currentImage];
    }

    public void ChangeScenes() {
      TransitionManager.Instance.MakeTransition(NextScene, NextSpawn);
    }

    #endregion
  }

}