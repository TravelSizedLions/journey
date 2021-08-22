using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [NodeWidth(300)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("Dialog/Character Reference")]
  public class CharacterNode : AutoValueNode {

    // AutoValueNode interface.
    public override object Value { get => Profile.CharacterName; }

    [Space(8)]
    [ValueDropdown("GetProfiles")]
    [SerializeField]
    [Tooltip("The character profile to reference.")]
    private CharacterProfile Profile;

    public override bool IsNodeComplete() {
      return base.IsNodeComplete() && 
             Profile != null;
    }

    //---------------------------------------------------------------------
    // Odin Inspector
    //---------------------------------------------------------------------
    public IEnumerable GetProfiles() {
      List<CharacterProfile> initialProfileList = new List<CharacterProfile>();

      #if UNITY_EDITOR
      initialProfileList = EditorUtils.FindAssetsByType<CharacterProfile>();
      #endif

      CharacterProfile defaultProfile = null;
      
      SortedDictionary<int, List<CharacterProfile>> separated = new SortedDictionary<int, List<CharacterProfile>>();
      foreach (CharacterProfile profile in initialProfileList) {
        if (profile.Category == CharacterCategory.Default) {
          defaultProfile = profile;
        } else {
          int cat = (int)profile.Category;
          if (!separated.ContainsKey(cat)) {
            separated.Add(cat, new List<CharacterProfile>());
          }
          
          separated[cat].Add(profile);
        }
      }
      
      ValueDropdownList<CharacterProfile> ordered = new ValueDropdownList<CharacterProfile>();
      ordered.Add("Default", defaultProfile);
      foreach (int cat in separated.Keys) {
        string categoryName = ((CharacterCategory)cat).ToString();
        foreach (CharacterProfile profile in separated[cat]) {
          string path = categoryName + "/" + profile.CharacterName;
          ordered.Add(path, profile);
        }
      }

      return ordered;
    }
  }
}