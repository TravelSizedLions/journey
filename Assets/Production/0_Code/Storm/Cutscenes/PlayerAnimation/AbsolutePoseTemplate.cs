using System;
using Storm.Characters;
using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  /// <summary>
  /// Information used for absolute positioning.
  /// </summary>
  /// <remarks>
  /// This sub-type is necessary to identify between absolute and relative clips
  /// in the Track Mixer for the player character. See 
  /// <see cref="PoseMixer.MixClips" /> to get a better idea of why this is necessary.
  /// </remarks>
  [Serializable]
  public class AbsolutePoseTemplate : PoseTemplate { }
}