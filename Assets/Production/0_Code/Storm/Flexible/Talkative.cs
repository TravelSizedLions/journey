using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Subsystems.Dialog;
using Storm.Characters.Player;

namespace Storm.Flexible {
  public class Talkative : MonoBehaviour {
    public DialogGraph dialog;


    private void OnTriggerEnter2D(Collider2D other) {

      // If the player is in the trigger area
      if (other.CompareTag("Player")) {
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        player.DisableJump();
        player.AddIndicator("QuestionMark");
        DialogManager.Instance.SetCanStartConversation(true);
        DialogManager.Instance.SetCurrentDialog(dialog);
      }
    }


    private void OnTriggerExit2D(Collider2D other) {
      // If the player has left the trigger area
      if (other.CompareTag("Player") && !DialogManager.Instance.IsInConversation) {
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        player.EnableJump();
        player.RemoveIndicator();
        DialogManager.Instance.SetCanStartConversation(false);
      }
    }
  }
}