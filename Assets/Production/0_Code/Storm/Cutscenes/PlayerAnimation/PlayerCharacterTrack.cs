using Sirenix.OdinInspector;
using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Storm.Cutscenes {
  /// <summary>
  /// A class that creates
  /// 
  /// </summary>
  [TrackClipType(typeof(PoseClip))]
  [TrackBindingType(typeof(PlayerCharacter))]
  public class PlayerCharacterTrack : TrackAsset {

    /// <summary>
    /// How to handle the end of a track.
    /// 
    /// <list type="bullet">
    /// <item> Resume - Resume normal play from where the track put the player. </item>
    /// <item> Freeze - Freeze the player in place where the track put them.</item>
    /// <item> Revert - Move the player back to their original state prior to the track.</item>
    /// <item> LoadScene - Load a new scene.</item>
    /// </list>
    /// </summary>
    [Tooltip("How to handle the end of the track.\n\nResume: Resume normal play where the track ends.\nFreeze: Freeze the player where the track ends.\nRevert: Revent the player to their state prior to the track.\nLoad Scene: Load a new unity scene.")]
    public OutroSetting Outro;

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
      ScriptPlayable<PlayerCharacterTrackMixer> mixerScript = ScriptPlayable<PlayerCharacterTrackMixer>.Create(graph, inputCount);
      
      PlayerCharacterTrackMixer mixer = mixerScript.GetBehaviour();
      mixer.Outro = Outro;

      return mixerScript;
    }
  }
}