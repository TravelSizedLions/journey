using System;

namespace HumanBuilders {
  /// <summary>
  /// Information used for relative positioning.
  /// </summary>
  /// <remarks>
  /// This sub-type is necessary to identify between absolute and absolute clips
  /// in the Track Mixer for the player character. See 
  /// <see cref="PlayerCharacterTrackMixer.MixMultiple" /> to get a better idea of why this is necessary.
  /// </remarks>
  [Serializable]
  public class RelativePoseInfo : PoseInfo { }
}