using System;
using Sirenix.OdinInspector;
using Storm.Characters.Player;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Storm.UI {

  /// <summary>
  /// A class representing a custom mouse icon.
  /// </summary>
  [Serializable]
  public class MouseIcon {


    #region Constants
    //-------------------------------------------------------------------------
    // Constants
    //-------------------------------------------------------------------------


    // The minimum height required to use a 128x128 cursor
    private static int SIZE_128_BOUNDARY = 2400;

    // The minimum height required to use a 64x64 cursor
    private static int SIZE_64_BOUNDARY = 1400;

    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The sprite for the icon (32x32 version).
    /// </summary>
    [Tooltip("The sprite for the icon (32x32).")]
    [PreviewField(32, ObjectFieldAlignment.Center)]
    [TableColumnWidth(64, false)]
    public Texture2D Sprite32;

    /// <summary>
    /// The sprite for the icon.
    /// </summary>
    [Tooltip("The sprite for the icon (64x64).")]
    [PreviewField(32, ObjectFieldAlignment.Center)]
    [TableColumnWidth(64, false)]
    public Texture2D Sprite64;

    /// <summary>
    /// The sprite for the icon.
    /// </summary>
    [Tooltip("The sprite for the icon (128x128).")]
    [PreviewField(32, ObjectFieldAlignment.Center)]
    [TableColumnWidth(64, false)]
    public Texture2D Sprite128;


    /// <summary>
    /// The name of the mouse icon.
    /// </summary>
    [Tooltip("The name of the mouse icon.")]
    public string Name;

    #endregion

    
    #region Public API
    //-------------------------------------------------------------------------
    // Public API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Swap in this mouse icon.
    /// </summary>
    public void Swap() {
      Texture2D icon = GetSprite();
      Vector2 hotspot = new Vector2(icon.width/2, icon.height/2);
      Cursor.SetCursor(icon, hotspot, CursorMode.ForceSoftware);
    }

    #endregion


    #region Helper Method
    //-------------------------------------------------------------------------
    // Helper Method
    //-------------------------------------------------------------------------

    /// <summary>
    /// Dynamically resize a mouse icon based on the system's resolution.
    /// </summary>
    /// <param name="mouseIcon">The source texture.</param>
    /// <returns>The resized texture</returns>
    private Texture2D GetSprite() {
      int displayHeight = Display.main.systemHeight;

      if (displayHeight < SIZE_64_BOUNDARY) {
        return Sprite32;
      } else if (displayHeight < SIZE_128_BOUNDARY) {
        return Sprite64;
      } else {
        return Sprite128;
      }
    }

    #endregion
  }

}