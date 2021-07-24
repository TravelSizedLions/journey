using System;
using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {
  public class ConditionListener : MonoBehaviour, IObserver<Variable> {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    public bool TestOnAwake = false;
    public VCondition Condition;
    [Space(10)]
    public UnityEvent PassActions;
    public UnityEvent FailActions;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    public void Awake() {
      if (TestOnAwake) {
        TestCondition();
      }

      if (Condition != null && Condition.Variable != null) {
        Condition.Variable.Subscribe(this);        
      }
    }

    public void OnApplicationQuit() {
      if (Condition != null && Condition.Variable != null) {
        Condition.Variable.Unsubscribe(this);        
      }
    }

    //-------------------------------------------------------------------------
    // Observer API
    //-------------------------------------------------------------------------
    public void OnCompleted() {}
    public void OnError(Exception error) {}
    public void OnNext(Variable value) {
      TestCondition();
    }

    private void TestCondition() {
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