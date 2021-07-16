
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [CreateAssetMenu(fileName = "New VSetter", menuName = "VSave/VSetter")]
  public class VSetter : ScriptableObject {
    [ShowInInspector]
    public IVariable Variable;

    [ShowIf("VType", VariableType.Boolean)]
    public bool BoolValue;

    [ShowIf("VType", VariableType.Float)]
    public float FloatValue;

    [ShowIf("VType", VariableType.Integer)]
    public int IntegerValue;

    [ShowIf("VType", VariableType.String)]
    public string StringValue;

    [ShowIf("VType", VariableType.GUID)]
    public GuidReference GUIDValue;

    public void Set() {
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

      Debug.Log("value: " + value);

      Variable.Value = value;
    }

    private VariableType VType() {
      return (Variable != null) ? Variable.Type : VariableType.Boolean;
    }
  }
}