
using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  public class ResumeStateMachine : PlayableBehaviour {
    public override void OnBehaviourPlay(Playable playable, FrameData info) {
      if (GameManager.Player != null && GameManager.Player.FSM != null && GameManager.Player.Animator != null) {
        GameManager.Player.FSM.Resume();
        GameManager.Player.Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
      }
    }
  }
}