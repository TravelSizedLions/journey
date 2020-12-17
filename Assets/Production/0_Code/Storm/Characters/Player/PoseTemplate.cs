using System;
using Sirenix.OdinInspector;
using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  [Serializable]
  public class PoseTemplate : PlayableBehaviour {

    public Type State;

    public AnimationClipPlayable Clip;
    public Vector3 Position;
    public bool Flipped;
    public override void OnBehaviourPlay(Playable playable, FrameData info) {
      StateDriver driver = StateDriver.For(State);

      if (GameManager.Player != null && GameManager.Player.FSM != null && GameManager.Player.Animator != null) {
        driver.ForceStateChangeOn(GameManager.Player.FSM);
      }
    }
  }
}