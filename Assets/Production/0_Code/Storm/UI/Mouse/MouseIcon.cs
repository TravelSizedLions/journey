using System;
using Sirenix.OdinInspector;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.UI {

  /// <summary>
  /// A class representing a custom mouse icon.
  /// </summary>
  [Serializable]
  public class MouseIcon {
    /// <summary>
    /// The sprite for the icon.
    /// </summary>
    [Tooltip("The sprite for the icon.")]
    [PreviewField(32, ObjectFieldAlignment.Center)]
    [TableColumnWidth(40, false)]
    public Texture2D Sprite;


    /// <summary>
    /// The name of the mouse icon.
    /// </summary>
    [Tooltip("The name of the mouse icon.")]
    public string Name;

    /// <summary>
    /// Swap in this mouse icon.
    /// </summary>
    public void Swap() {
      Debug.Log("New Cursor: " + Sprite);
      Cursor.SetCursor(Sprite, new Vector2(16, 16), CursorMode.ForceSoftware);
    }
  }

}