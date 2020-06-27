using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using Storm.Flexible.Interaction;
using Storm.Subsystems.Dialog;
using UnityEngine;

namespace Storm.Flexible {
  public class Talkative : Interactible {
    public DialogGraph dialog;

    /// <summary>
    /// What the object should do when interacted with.
    /// </summary>
    public override void OnInteract() {
      Debug.Log("Dialog!");
      if (!interacting) {
        interacting = true;
        DialogManager.Instance.StartDialog(dialog);
      } else {
        DialogManager.Instance.ContinueDialog();
        if (DialogManager.Instance.IsDialogFinished()) {
          DialogManager.Instance.EndDialog();
          interacting = false;
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