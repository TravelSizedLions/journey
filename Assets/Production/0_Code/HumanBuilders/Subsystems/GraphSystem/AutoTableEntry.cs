
using System;

namespace HumanBuilders {

  public interface IAutoTableEntry {
    Object Value { get; set; }
  }

  public abstract class AutoTableEntry : UnityEngine.Object, IAutoTableEntry {
    public Object Value { get; set; }
  }

  [Serializable]
  public class AutoTableEntry<T> : AutoTableEntry {
    public new T Value { get; set; }

    public AutoTableEntry() {}
    public AutoTableEntry(T value) {
      Value = value;
    }
  }
}