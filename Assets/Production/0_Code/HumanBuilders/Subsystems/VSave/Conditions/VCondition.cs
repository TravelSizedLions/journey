using System;
using Sirenix.OdinInspector;
using UnityEngine.GUID;

namespace HumanBuilders {
  [Serializable]
  public class VCondition {

    public Variable Variable;

    public dynamic ExpectedValue {
      get {
        if (Variable == null) {
          return null;
        }

        switch (Variable.Type) {
          case VariableType.Boolean:
            return ExpectedBool;
          case VariableType.Integer:
            return ExpectedInteger;
          case VariableType.Float:
            return ExpectedFloat;
          case VariableType.String:
            return ExpectedString;
          case VariableType.GUID:
            return ExpectedGUID;
        }

        return null;
      }

      set {
        switch (Variable.Type) {
          case VariableType.Boolean:
            ExpectedBool = value;
            return;
          case VariableType.Integer:
            ExpectedInteger = value;
            return;
          case VariableType.Float:
            ExpectedFloat = value;
            return;
          case VariableType.String:
            ExpectedString = value;
            return;
          case VariableType.GUID:
            ExpectedGUID = value;
            return;
        }
      }
    }

    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.Boolean)]
    [ShowInInspector]
    [LabelText("Expected")]
    public bool ExpectedBool = true;

    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.Float)]
    [LabelText("Expected")]
    public float ExpectedFloat;

    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.Integer)]
    [LabelText("Expected")]
    public int ExpectedInteger;

    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.String)]
    [LabelText("Expected")]
    public string ExpectedString;

    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.GUID)]
    [LabelText("Expected")]
    public GuidReference ExpectedGUID;

    public bool IsMet() {
      return Variable?.Value == ExpectedValue;
    }

    private VariableType VType() => (Variable != null) ? Variable.Type : VariableType.Boolean;
  }

}