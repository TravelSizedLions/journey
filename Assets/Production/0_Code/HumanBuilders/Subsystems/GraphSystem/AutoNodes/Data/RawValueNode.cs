using Sirenix.OdinInspector;
using UnityEngine.GUID;

namespace HumanBuilders.Graphing {
  [NodeWidth(360)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Data/Raw Value")]
  public class RawValueNode : AutoValueNode {
    public override object Value { 
      get {
        switch (Type) {
          case VariableType.Boolean:
            return BoolValue;
          case VariableType.Integer:
            return IntValue;
          case VariableType.Float:
            return FloatValue;
          case VariableType.String:
            return StringValue;
          case VariableType.GUID:
            return GUIDValue;
        }

        return null;
      }
    }

    public VariableType Type;

    [ShowIf("Type", VariableType.Boolean)]
    [LabelText("Value")]
    public bool BoolValue; 

    [ShowIf("Type", VariableType.Integer)]
    [LabelText("Value")]
    public int IntValue; 

    [ShowIf("Type", VariableType.Float)]
    [LabelText("Value")]
    public float FloatValue; 

    [ShowIf("Type", VariableType.String)]
    [LabelText("Value")]
    public string StringValue; 

    [ShowIf("Type", VariableType.GUID)]
    [LabelText("Value")]
    public GuidReference GUIDValue; 

  }
}