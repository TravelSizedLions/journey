
using Storm.Subsystems.FSM;
using UnityEngine.Timeline;

namespace Storm.Cutscenes {
  [TrackClipType(typeof(PlayerControlAsset))]
  [TrackClipType(typeof(ResumeStateMachineAsset))]
  [TrackClipType(typeof(PauseStateMachineAsset))]
  public class PlayerTrack : TrackAsset {}
}