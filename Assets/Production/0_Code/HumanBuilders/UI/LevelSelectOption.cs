using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace HumanBuilders {

  /// <summary>
  /// One level option on a level select screen.
  /// </summary>
  /// <seealso cref="LevelSelect" />
  [System.Serializable]
  public class LevelSelectOption {

    #region Variables

    [Header("Level Infomation", order = 0)]
    [Space(10, order = 1)]

    /// <summary>
    /// The name of the level.
    /// </summary>
    [Tooltip("The name of the level that displays on screen.")]
    [SerializeField]
    private string title = "";

    /// <summary>
    /// The image associated with the level.
    /// </summary>
    [Tooltip("The image associated with the level.")]
    [SerializeField]
    private Sprite levelImage = null;

    [Space(10, order = 3)]

    /// <summary>
    /// The name of the unity scene to load.
    /// </summary>
    [Tooltip("The name of the unity scene to load.")]
    [SerializeField]
    private string sceneName = "";
    #endregion

    #region Getters
    //-------------------------------------------------------------------------
    // Getters
    //-------------------------------------------------------------------------

    /// <summary>
    /// Get the title of the level.
    /// </summary>
    /// <returns>The title of the level</returns>
    public string GetTitle() {
      return title;
    }


    /// <summary>
    /// Get the image preview of the level.
    /// </summary>
    /// <returns>The sprite image preview.</returns>
    public Sprite GetLevelImage() {
      return levelImage;
    }


    /// <summary>
    /// Get the name of the unity scene to load.
    /// </summary>
    /// <returns>The unity scene to load. This doesn't need to be a path.</returns>
    public string GetSceneName() {
      return sceneName;
    }

    #endregion
  }
}