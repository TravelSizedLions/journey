using System;
using Storm.Characters;
using Storm.Characters.Player;
using Storm.Subsystems.FSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  [Serializable]
  public class PoseTemplate : PlayableBehaviour {
    public Type State;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale = Vector3.one;
    public bool Flipped;
  }
}