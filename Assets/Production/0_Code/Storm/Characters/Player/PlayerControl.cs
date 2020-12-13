using System;
using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  public class PlayerControl : PlayableBehaviour {

    public Type State;

    public AnimationClipPlayable Clip;

    public override void OnBehaviourPlay(Playable playable, FrameData info) {
      StateDriver driver = StateDriver.For(State);

      if (GameManager.Player != null && GameManager.Player.FSM != null && GameManager.Player.Animator != null) {
        driver.ForceStateChangeOn(GameManager.Player.FSM);
        AnimationPlayableUtilities.Play(GameManager.Player.Animator, Clip, playable.GetGraph());
      }

    }
  }
}