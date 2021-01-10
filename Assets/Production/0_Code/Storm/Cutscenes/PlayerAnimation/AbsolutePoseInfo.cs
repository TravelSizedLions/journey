using System;

namespace HumanBuilders {
  /// <summary>
  /// Information used for absolute positioning.
  /// </summary>
  /// <remarks>
  /// This sub-type is necessary to identify between absolute and relative clips
  /// in the Track Mixer for the player character. See 
  /// <see cref="PlayerCharacterTrackMixer.MixMultiple" /> to get a better idea of why this is necessary.
  /// </remarks>
  [Serializable]
  public class AbsolutePoseInfo : PoseInfo { }
}