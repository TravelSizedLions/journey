
using UnityEngine;

using Storm.Flexible.Interaction;
using Storm.Subsystems.Transitions;

namespace Storm.LevelMechanics.Doors {
  /// <summary>
  /// A door that lets the player change scenes (ala super mario brothers 2).
  /// </summary>
  public class TransitionDoor : Interactible {
    #region Fields
    [Header("Scene Change Info", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The name of the scene this doorway connects to. Does not need to be a full or relative path, or include the scene's file extension.
    /// </summary>
    [Tooltip("The name of the scene this doorway connects to. Does not need to be a full or relative path, or include the scene's file extension.")]
    [SerializeField]
    private string sceneName = "";

    /// <summary>
    /// The name of the spawn point the player will be placed at in the next scene.
    /// If none is specified, the player's spawn will be set to wherever the player 
    /// game object is currently located in-editor in the next scene.
    /// </summary>
    [Tooltip("The name of the spawn point the player will be placed at in the next scene.\nIf none is specified, the player's spawn will be set to wherever the player game object is currently located in-editor in the next scene.")]
    [SerializeField]
    private string spawnName = "";

    #endregion

    #region Interactible API
    /// <summary>
    /// What the object should do when interacted with.
    /// </summary>
    public override void OnInteract() {
      if (player != null) {
        TransitionManager.Instance.MakeTransition(sceneName, spawnName);
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
    #endregion
  }
}
