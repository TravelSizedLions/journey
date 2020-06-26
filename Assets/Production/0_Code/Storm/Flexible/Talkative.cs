using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using Storm.Flexible.Interaction;
using Storm.Subsystems.Dialog;
using UnityEngine;

namespace Storm.Flexible {
  public class Talkative : Interactible {
    public DialogGraph dialog;

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

    public override bool ShouldShowIndicator() {
      return true;
    }
  }
}