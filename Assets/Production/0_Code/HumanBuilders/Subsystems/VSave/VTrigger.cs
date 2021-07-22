
using System;
using System.Runtime.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [Serializable]
  public class VTrigger : Triggerable {
    [VerticalGroup("Trigger")]
    public Variable Variable;

    [ShowIf("VType", VariableType.Boolean)]
    [LabelText("Set To")]
    [VerticalGroup("Trigger")]
    public bool BoolValue;

    [ShowIf("VType", VariableType.Float)]
    [LabelText("Set To")]
    [VerticalGroup("Trigger")]
    public float FloatValue;

    [ShowIf("VType", VariableType.Integer)]
    [LabelText("Set To")]
    [VerticalGroup("Trigger")]
    public int IntegerValue;

    [ShowIf("VType", VariableType.String)]
    [LabelText("Set To")]
    [VerticalGroup("Trigger")]
    public string StringValue;

    [ShowIf("VType", VariableType.GUID)]
    [LabelText("Set To")]
    [VerticalGroup("Trigger")]
    public GuidReference GUIDValue;

    public override void Pull() {
      if (Variable == null) {
        return;
      }
      
      VariableType t = VType();
      dynamic value = null;
      switch (t) {
        case VariableType.Boolean: value = BoolValue; break;
        case VariableType.Float: value = FloatValue; break;
        case VariableType.Integer: value = IntegerValue; break;
        case VariableType.String: value = StringValue; break;
        case VariableType.GUID: value = GUIDValue; break;
      }

      Variable.Value = value;
    }

    private VariableType VType() {
      return (Variable != null) ? Variable.Type : VariableType.Boolean;
    }
  }
}