using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Subsystems.Dialog {
  public class SwitchDialog : MonoBehaviour {
    
    /// <summary>
    /// The target to switch the dialog on.
    /// </summary>
    [Tooltip("The target to switch the dialog on.")]
    public Talkative Target;

    [Space(10, order=0)]

    /// <summary>
    /// The dialog graph in the scene that will be used in the conversation. Use
    /// this instead of the asset dialog if you need the graph to reference
    /// objects in the scene. This will be used instead of the asset graph if
    /// both are populated.
    /// </summary>
    [Tooltip("The dialog graph in the scene that will be used in the conversation.\n\nUse this instead of the asset dialog if you need the graph to reference objects in the scene. This will be used instead of the asset graph if both are populated.")]
    public SceneDialogGraph SceneDialog;

    /// <summary>
    /// The dialog graph asset that will be used in the conversation. This does
    /// not support referencing objects in the scene. If both dialogs are
    /// populated, the scene dialog graph will be used instead.
    /// </summary>
    [Tooltip("The dialog graph asset that will be used in the conversation.\n\nThis does not support referencing objects in the scene. If both dialogs are populated, the scene dialog graph will be used instead.")]
    public DialogGraph AssetDialog;



    public void Switch() {
      if (SceneDialog != null) {
        Target.SceneDialog = SceneDialog;
      } else {
        Target.AssetDialog = AssetDialog;
      }
    }
  }
}