using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {
  public enum CombinationLogic { AND, OR }
  public class MultiConditionListener : MonoBehaviour, IObserver<Variable> {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    public bool TestOnAwake = false;
    public CombinationLogic Logic;
    public List<VCondition> Conditions;
    [Space(10)]
    public UnityEvent PassActions;
    public UnityEvent FailActions;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    public void Awake() {
      if (TestOnAwake) {
        TestConditions();
      }

      if (Conditions != null) {
        foreach (var cond in Conditions) {
          if (cond.Variable != null) {
            cond.Variable.Subscribe(this);
          }
        }
      }
    }

    public void OnApplicationQuit() {
      if (Conditions != null) {
        foreach (var cond in Conditions) {
          if (cond.Variable != null) {
            cond.Variable.Unsubscribe(this);
          }
        }
      }
    }

    //-------------------------------------------------------------------------
    // Observer API
    //-------------------------------------------------------------------------
    public void OnCompleted() {}
    public void OnError(Exception error) {}
    public void OnNext(Variable value) {
      TestConditions();
    }

    private void TestConditions() {
      bool pass = (Logic == CombinationLogic.AND) ? true : false;

      if (Conditions != null) {
        foreach (var cond in Conditions) {
          switch (Logic) {
            case CombinationLogic.AND:
              pass &= cond.IsMet();
              break;
            case CombinationLogic.OR:
              pass |= cond.IsMet();
              break;
          }

          if (!pass && Logic == CombinationLogic.AND) {
            break;
          }
        }
      }

      if (pass) {
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