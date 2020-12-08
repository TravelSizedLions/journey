using System.Collections;
using System.Collections.Generic;
using Storm;
using Storm.Characters.Player;
using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  public class InGameCutscene : MonoBehaviour {
    private PlayableDirector director;

    private void Awake() {
      director = GetComponent<PlayableDirector>();
    }


    public void TogglePlayerCutsceneMode(bool enable) {
      if (enable) {
        GameManager.Player.EnableCutsceneMode();
      } else {
        GameManager.Player.DisableCutsceneMode();
      }
    }
  }
}
