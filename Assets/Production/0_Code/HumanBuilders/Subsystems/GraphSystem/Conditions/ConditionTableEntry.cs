
using System;
using Sirenix.OdinInspector;

namespace HumanBuilders {
  [Serializable]
  public class ConditionTableEntry {
    [ShowInInspector]
    public ICondition Condition;

    public ConditionTableEntry() {

    }
    
    public ConditionTableEntry(ICondition condition) {
      Condition = condition;
    }
  }
}