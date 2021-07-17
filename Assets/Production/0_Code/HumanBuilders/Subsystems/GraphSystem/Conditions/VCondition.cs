using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace HumanBuilders {
  [CreateAssetMenu(fileName = "New Condition", menuName = "Conditions/VCondition")]
  public class VCondition : ScriptableCondition {

    [ShowInInspector]
    public IVariable Variable;

    public dynamic ExpectedValue {
      get {
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

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.Boolean)]
    [ShowInInspector]
    public bool ExpectedBool { get; set; } = true;

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.Float)]
    [ShowInInspector]
    public float ExpectedFloat { get; set; }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.Integer)]
    [ShowInInspector]
    public int ExpectedInteger { get; set; }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.String)]
    [ShowInInspector]
    public string ExpectedString { get; set; }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("VType", VariableType.GUID)]
    [ShowInInspector]
    public GuidReference ExpectedGUID { get; set; }

    public override bool IsMet() {
      return Variable.Value == ExpectedValue;
    }

    private VariableType VType() => (Variable != null) ? Variable.Type : VariableType.Boolean;
  }

}