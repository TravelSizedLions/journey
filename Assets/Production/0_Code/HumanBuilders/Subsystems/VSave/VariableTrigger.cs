using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  public class VariableTrigger : MonoBehaviour, ITriggerable {
    [AutoTable(typeof(VTrigger))]
    public List<VTrigger> Triggers;

    public void Pull() {
      if (Triggers != null) {
        foreach (var trigger in Triggers) {
          trigger.Pull();
        }
      }
    }
  }
}