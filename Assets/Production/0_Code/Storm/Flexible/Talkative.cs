using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using Storm.Flexible.Interaction;
using Storm.Subsystems.Dialog;
using UnityEngine;

namespace Storm.Flexible {
  public class Talkative : Interactible {

    [Header("Dialog Setup", order=0)]
    [Space(5, order=1)]

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

    protected new void Awake() {
      base.Awake();

      if (AssetDialog != null && SceneDialog != null) {
        Debug.LogWarning("Talkative object \"" + gameObject.name + "\" should only have one dialog graph attached. The scene dialog will be preferred over the asset dialog.");
      }
    }

    protected new void OnDestroy() {
      base.OnDestroy();
      if (DialogManager.Instance != null) {
        DialogManager.Instance.EndDialog();
      }
    }

    /// <summary>
    /// What the object should do when interacted with.
    /// </summary>
    public override void OnInteract() {
      if (!interacting) {
        interacting = true;
        if (SceneDialog != null) {
          DialogManager.Instance.StartDialog(SceneDialog.graph);
        } else {
          DialogManager.Instance.StartDialog(AssetDialog);
        }

      } else {
        DialogManager.Instance.ContinueDialog();
        if (DialogManager.Instance.IsDialogFinished()) {
          interacting = false;

          // This stops the player from hopping/twitching after the conversation
          // ends.
          Input.ResetInputAxes();
        }
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
  }
}