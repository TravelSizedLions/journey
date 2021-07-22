using System;
using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {
  public class ConditionListener : MonoBehaviour, IObserver<Variable> {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    public VCondition Condition;
    [Space(10)]
    public UnityEvent PassActions;
    public UnityEvent FailActions;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      if (Condition != null && Condition.Variable != null) {
        Debug.Log("Subscribing");
        Condition.Variable.Subscribe(this);        
      }
    }

    //-------------------------------------------------------------------------
    // Observer API
    //-------------------------------------------------------------------------
    public void OnCompleted() {}
    public void OnError(Exception error) {}
    public void OnNext(Variable value) {
      Debug.Log("Check condition");
      if (Condition.IsMet()) {
        if (PassActions != null) {
          PassActions.Invoke();
        }
      } else {
        if (FailActions != null) {
          FailActions.Invoke();
        }
      }
    }
  }
}