using System.Collections;
using System.Collections.Generic;
using Storm;
using Storm.Characters.Player;
using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  public class TimelineManager : MonoBehaviour {
    private PlayableDirector director;

    private void Awake() {
      director = GetComponent<PlayableDirector>();
    }

    private void OnEnable() {
      GameManager.Player.EnableCutsceneMode();
    }

    private void Update() {
      if (director.state != PlayState.Playing) {
        GameManager.Player.DisableCutsceneMode();
      }
    }
  }
}
