
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  public abstract class AutoTableEntry : UnityEngine.Object {
    public UnityEngine.Object Value;

  }

  [Serializable]
  public class AutoTableEntry<T> : AutoTableEntry {
    [InlineEditor]
    public new T Value;

    public AutoTableEntry() {}
    public AutoTableEntry(T value) {
      Value = value;
    }
  }
}