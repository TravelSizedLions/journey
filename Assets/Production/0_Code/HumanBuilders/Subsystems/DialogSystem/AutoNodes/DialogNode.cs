using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A node representing a single screen of text with a speaker.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Dialog/Sentence")]
  public class DialogNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The character profile to use for this dialog.
    /// </summary>
    [ValueDropdown("GetProfiles")]
    public CharacterProfile Profile;

    /// <summary>
    /// Whether or not to show the speaker's name in the dialog.
    /// </summary>
    public bool ShowSpeaker = true;

    [Space(8, order=1)]

    /// <summary>
    /// The text being spoken.
    /// </summary>
    [TextArea(3,10)]
    public string Text;

    [Space(8)]

    /// <summary>
    /// Whether or not to wait for the the player to advance the dialog.
    /// </summary>
    [Tooltip("Whether or not to wait for the the player to advance the dialog.")]
    public bool AutoAdvance;

    [Space(8)]

    /// <summary>
    /// How long to wait before advancing automatically.
    /// </summary>
    [ShowIf("AutoAdvance")]
    public float Delay;

    [Space(8, order=1)]

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    public override void Handle(GraphEngine graphEngine) {
      if (!DialogManager.IsDialogBoxOpen()) {
        DialogManager.OpenDialogBox();
      }

      if (Profile == null && Profile.UseDefaultColors) {
        DialogManager.UseDefaultDialogColors();
      } else {
        DialogManager.UseCharacterProfile(Profile);
      }
      
      string speaker = (Profile != null) ? Profile.CharacterName : "";
      DialogManager.Type(Text, speaker, AutoAdvance, Delay);
    }

    public override void PostHandle(GraphEngine graphEngine) {}

    public override bool IsComplete() {
      return base.IsComplete() && 
             Profile != null && 
             !string.IsNullOrEmpty(Text);
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