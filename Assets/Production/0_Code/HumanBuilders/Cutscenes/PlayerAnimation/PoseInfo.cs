using System;
using UnityEngine;
using UnityEngine.Playables;

namespace HumanBuilders {
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