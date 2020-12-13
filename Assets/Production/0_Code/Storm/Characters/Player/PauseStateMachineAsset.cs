using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  public class PauseStateMachineAsset : PlayableAsset {
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
      return ScriptPlayable<PauseStateMachine>.Create(graph);
    }

  }
}