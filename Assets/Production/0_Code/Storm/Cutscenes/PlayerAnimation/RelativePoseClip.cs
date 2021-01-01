using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using System;


namespace Storm.Cutscenes {
  /// <summary>
  /// A class representing a player pose clip for animating the player
  /// character's transform relative to the player's last known absolute position.
  /// </summary>
  /// <seealso cref="AbsolutePoseClip" />
  /// <seealso cref="PlayerCharacterTrack" />
  /// <seealso cref="RelativePoseInfo" />
  public class RelativePoseClip : PoseClip {

    /// <summary>
    /// The template for absolute posing clips.
    /// </summary>
    /// <remarks>
    /// While it's tempting to pull up this variable and it's sister variable in
    /// AbsolutePose.cs into Pose.cs, these templates are the only way for the
    /// track mixer to identify which type of clips it's mixing together (see
    /// <see cref="PlayerCharacterTrackMixer.MixClips"/>).
    /// </remarks>
    public RelativePoseInfo template;

    /// <summary>
    /// Create the script playable for the pose type.
    /// </summary>
    public override ScriptPlayable<PoseInfo> CreateScriptPlayable(PlayableGraph graph)  => ScriptPlayable<PoseInfo>.Create(graph, template);
  }
}