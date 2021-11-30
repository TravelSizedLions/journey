using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {
  public class OnAwake: MonoBehaviour {

    [Tooltip("The events you'd like to fire *every time* this script calls Awake()")]
    public UnityEvent Events;
    
    public void Awake() {
      if (Events != null) {
        Events.Invoke();
      }
    }
  }
}