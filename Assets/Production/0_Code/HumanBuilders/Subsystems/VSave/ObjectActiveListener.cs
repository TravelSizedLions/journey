
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public enum ToggleOption { ToggleOn, ToggleOff }
  public class ObjectActiveListener : MonoBehaviour, IObserver<Variable> {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [LabelWidth(150)]
    public bool TestOnAwake = true;

    [LabelWidth(150)]
    public CombinationLogic Logic;

    [LabelText("When Conditions Pass:")]
    [LabelWidth(150)]
    public ToggleOption OnPass;

    [Space(10)]
    public List<VCondition> Conditions;

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
      Unsubcribe();
    }

    public void OnDestroy() {
      Unsubcribe();
    }

    private void Unsubcribe() {
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

      bool isActive = (OnPass == ToggleOption.ToggleOn) ? pass : !pass;
      gameObject.SetActive(isActive);
    }
  }
}