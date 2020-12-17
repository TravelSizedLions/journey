using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Storm.Cutscenes {
  [TrackClipType(typeof(Pose))]
  [TrackClipType(typeof(ResumeStateMachineAsset))]
  [TrackClipType(typeof(PauseStateMachineAsset))]
  [TrackBindingType(typeof(PlayerCharacter))]
  public class PlayerTrack : TrackAsset {
    public bool KeepStateAfterPlay;

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
      ScriptPlayable<PoseMixer> mixerScript = ScriptPlayable<PoseMixer>.Create(graph, inputCount);
      
      PoseMixer mixer = mixerScript.GetBehaviour();
      mixer.KeepStateAfterPlay = KeepStateAfterPlay;

      return mixerScript;
    }
  }
}