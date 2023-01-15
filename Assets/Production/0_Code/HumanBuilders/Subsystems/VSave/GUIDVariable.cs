
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.GUID;

namespace HumanBuilders {
  [CreateAssetMenu(fileName = "New VSave GUID Variable", menuName = "Variable/GUID V-Variable")]
  public class GUIDVariable :  Variable {

    [BoxGroup("Locator")]
    [OdinSerialize]
    public GuidReference GUID;


    public override string Key { 
      get => UseCustomKey ? GUID.ToString()+CustomKey : GUID.ToString()+PresetKey; 
      set { if (UseCustomKey) CustomKey = value; else PresetKey = value; }
    }
  }
}