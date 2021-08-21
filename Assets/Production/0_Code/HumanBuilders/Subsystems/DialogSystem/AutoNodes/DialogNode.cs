using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  /// <summary>
  /// A node representing a single screen of text with a speaker.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Dialog/Sentence")]
  public class DialogNode : AutoNode {

    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(5)]
    [PropertyOrder(998)]
    [ShowIf("UseFormatting")]
    [Input(dynamicPortList=true, connectionType=ConnectionType.Multiple)]
    public List<EmptyConnection> FormattedValues;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [Space(8)]
    [ValueDropdown("GetProfiles")]
    [LabelWidth(105)]
    public CharacterProfile Profile;

    [LabelWidth(105)]
    public bool ShowSpeaker = true;

    [LabelWidth(105)]
    public bool AnimateSpeaker = true;

    [Space(8)]
    [TextArea(3,10)]
    public string Text;

    [Space(8)]
    [LabelWidth(105)]
    [Tooltip("Whether or not to wait for the the player to advance the dialog.")]
    public bool AutoAdvance;

    [Space(8)]
    [ShowIf("AutoAdvance")]
    [LabelWidth(105)]
    [Tooltip("How long to wait before advancing conversation automatically.")]
    public float Delay;

    [Tooltip("Use dynamically formatted values.")]
    [LabelWidth(105)]
    public bool UseFormatting;


    public override void Handle(GraphEngine graphEngine) {
      if (!DialogManager.IsDialogBoxOpen()) {
        DialogManager.OpenDialogBox();
      }

      SwapCharacterProfile();
      
      string speaker = (Profile != null) ? Profile.CharacterName : "";
      string text = Text;
      Debug.Log("UseFormatting: " + UseFormatting);
      Debug.Log("FormattedValues.Count: " + FormattedValues.Count);
      if (UseFormatting && FormattedValues.Count > 0) {
        text = GetFormattedSentence();
      }

      DialogManager.Type(text, speaker, AutoAdvance, Delay, AnimateSpeaker);
    }

    public override void PostHandle(GraphEngine graphEngine) {}

    public override bool IsNodeComplete() {
      if (Profile == null || string.IsNullOrEmpty(Text)) {
        return false;
      }

      foreach (NodePort port in Ports) {
        if (!port.IsConnected || port.GetConnections().Count == 0) {
          if (port.fieldName.Contains("FormattedValues")) {
            continue;
          }

          return false;
        }
      }

      if (UseFormatting) {
        for (int i = 0; i < FormattedValues.Count; i ++) {
          NodePort port = GetInputPort(string.Format("FormattedValues {0}", i));
          if (!port.IsConnected) {
            return false;
          }
        }
      }

      return true;
    }

    private void SwapCharacterProfile() {
      if (Profile == null) {
        DialogManager.UseDefaultDialogColors();
      } else {
        DialogManager.UseCharacterProfile(Profile);
        if (Profile.UseDefaultColors) {
          DialogManager.UseDefaultDialogColors();
        }
      }
    }

    private string GetFormattedSentence() {
      List<object> args = new List<object>();
      for (int i = 0; i < FormattedValues.Count; i++) {
        NodePort inPort = GetInputPort("FormattedValues "+i);
        NodePort outPort = inPort.Connection;

        if (outPort.node is AutoValueNode n) {
          args.Add(n.Value);
        } else {
          Debug.LogWarning("Unrecognized value node: \'" + outPort.node.GetType() + "\'");
        }
      }

      return string.Format(Text, args.ToArray());
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