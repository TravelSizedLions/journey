
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [CreateAssetMenu(fileName = "New VSave GUID Variable", menuName = "VSave/GUID Variable")]
  public class GUIDVariable :  Variable {

    [BoxGroup("Locator")]
    [ShowInInspector]
    public GuidReference GUID { get; set; }


    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.Boolean)]
    [ShowInInspector]
    public override bool BoolValue {
      get => VSave.Get<bool>(Folder, Key);
      set => VSave.Set(Folder, GUID.ToString()+"_"+Key, value);
    }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.Float)]
    [ShowInInspector]
    public override float FloatValue {
      get => VSave.Get<float>(Folder, Key);
      set => VSave.Set(Folder, GUID.ToString()+"_"+Key, value);
    }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.Integer)]
    [ShowInInspector]
    public override int IntegerValue {
      get => VSave.Get<int>(Folder, Key);
      set => VSave.Set(Folder, GUID.ToString()+"_"+Key, value);
    }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.String)]
    [ShowInInspector]
    public override string StringValue {
      get => VSave.Get<string>(Folder, Key);
      set => VSave.Set(Folder, GUID.ToString()+"_"+Key, value);
    }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.GUID)]
    [ShowInInspector]
    public override GuidReference GUIDValue {
      get {
        string guid = GUID?.ToString();
        if (!string.IsNullOrEmpty(guid)) {
          return null;
        }

        if (VSave.Get(Folder, guid+"_"+Key, out byte[] bytes) && bytes != null) {
          return new GuidReference(bytes);
        }

        return null;
      }
      set => VSave.Set(Folder, GUID.ToString()+"_"+Key, value.ToByteArray());
    }
  }
}