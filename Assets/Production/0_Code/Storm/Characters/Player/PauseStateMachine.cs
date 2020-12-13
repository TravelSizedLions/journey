using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Storm.Cutscenes {
  public class PauseStateMachine : PlayableBehaviour {
    public override void OnBehaviourPlay(Playable playable, FrameData info) {
      if (GameManager.Player != null && GameManager.Player.FSM != null && GameManager.Player.Animator != null) {
        Debug.Log("Pausing FSM!");
        GameManager.Player.FSM.Pause();
        GameManager.Player.Animator.updateMode = AnimatorUpdateMode.Normal;
      }
    }
  }
}