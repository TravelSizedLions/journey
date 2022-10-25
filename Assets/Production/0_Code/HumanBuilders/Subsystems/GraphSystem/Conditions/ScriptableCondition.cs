using System;
using UnityEngine;
using XNode;

namespace HumanBuilders.Graphing {
  [Serializable]
  public class ScriptableCondition : ScriptableObject, ICondition {
    public virtual bool IsMet() => false;
  }
}