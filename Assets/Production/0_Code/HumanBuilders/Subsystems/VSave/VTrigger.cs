
using System;
using System.Runtime.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [Serializable]
  [CreateAssetMenu(fileName = "New Trigger", menuName = "VSave/Trigger")]
  public class VTrigger : Triggerable {
    [ShowInInspector]
    public IVariable Variable;

    [ShowIf("VType", VariableType.Boolean)]
    [LabelText("Set To")]
    public bool BoolValue;

    [ShowIf("VType", VariableType.Float)]
    [LabelText("Set To")]
    public float FloatValue;

    [ShowIf("VType", VariableType.Integer)]
    [LabelText("Set To")]
    public int IntegerValue;

    [ShowIf("VType", VariableType.String)]
    [LabelText("Set To")]
    public string StringValue;

    [ShowIf("VType", VariableType.GUID)]
    [LabelText("Set To")]
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