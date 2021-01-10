using System;
using UnityEngine;
using UnityEngine.Playables;

namespace HumanBuilders {

  /// <summary>
  /// This class stores and passes information about the player character to the
  /// <see cref="PlayerCharacterTrackMixer" /> for use during Timeline playback.
  /// 
  /// See the <see cref="PlayerSnapshot" /> class for more details about each of
  /// these fields.
  /// </summary>
  [Serializable]
  public class PoseInfo : PlayableBehaviour {
    public Type State;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale = Vector3.one;
    public bool Flipped;
    public bool Active = true;
  }
}