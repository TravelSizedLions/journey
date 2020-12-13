using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Storm.Cutscenes {
  public class ResumeStateMachineAsset : PlayableAsset {

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
      return ScriptPlayable<ResumeStateMachine>.Create(graph);
    }

  }
}