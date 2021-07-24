using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using XNode;

#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace HumanBuilders {

  [CreateNodeMenu("Quest")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [Serializable]
  public class QuestNode : JourneyNode, ISkippable {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [Input(connectionType = ConnectionType.Multiple)]
    [PropertyOrder(0)]
    public EmptyConnection Input;

    [Output(connectionType = ConnectionType.Multiple)]
    [ShowIf("Required")]
    [PropertyOrder(999)]
    [PropertySpace(10)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [ShowInInspector]
    [PropertyTooltip("The player facing quest title")]
    [OnValueChanged("OnTitleChanged")]
    public string Title {
      get => Quest != null ? Quest.Title : title;
      set {
        if (Quest != null) {
          Quest.Title = value;
        } else {
          title = value;
        }
      }
    }

    protected string title;

    [OnValueChanged("OnQuestChange")]
    [PropertyOrder(1)]
    [OdinSerialize]
    public QuestGraph Quest;

    [PropertyOrder(2)]
    [ShowInInspector]
    public bool Required { 
      get => (Quest != null) ? Quest.Required : false; 
      set {
        if (Quest != null) {
          Quest.Required = value;
        }
      }
    }

    [PropertyOrder(2)]
    [ShowInInspector]
    [HideIf("Required")]
    [PropertyTooltip("Check this if you want this quest to be skipped\nwhen its parent quest is completed.")]
    public bool Missable { 
      get => (Quest != null) ? Quest.Missable : false; 
      set {
        if (Quest != null) {
          Quest.Missable = value;
        }
      }
    }

    // --- Rewards ---
    [PropertyOrder(3)]
    [ShowInInspector]
    [TitleGroup("Rewards")]
    [ShowIf("QuestPresent")]
    [AutoTable(typeof(VTrigger), "Quest Rewards", NodeColors.BASIC_COLOR)]
    public List<VTrigger> Rewards {
      get => Quest?.Rewards;
      set { if (Quest != null) Quest.Rewards = value; }
    }

    // --- Conditions ---
    [ShowInInspector]
    [PropertyOrder(4)]
    [TitleGroup("Progress Conditions")]
    [ShowIf("QuestPresent")]
    [AutoTable(typeof(VCondition), "Availability Prerequisites", NodeColors.BASIC_COLOR)]
    public List<VCondition> AvailabilityConditions {
      get => Quest?.AvailabilityConditions;
      set { if (Quest != null) Quest.AvailabilityConditions = value; }
    }

    [ShowInInspector]
    [PropertyOrder(5)]
    [TitleGroup("Progress Conditions")]
    [ShowIf("QuestPresent")]
    [AutoTable(typeof(VCondition), "Start Prerequisites", NodeColors.BASIC_COLOR)]
    public List<VCondition> StartConditions {
      get => Quest?.StartConditions;
      set { if (Quest != null) Quest.StartConditions = value; }
    }

    [ShowInInspector]
    [PropertyOrder(6)]
    [TitleGroup("Progress Conditions")]
    [ShowIf("QuestPresent")]
    [AutoTable(typeof(VCondition), "Completion Prerequisites", NodeColors.BASIC_COLOR)]
    public List<VCondition> CompletionConditions {
      get => Quest?.CompletionConditions;
      set { if (Quest != null) Quest.CompletionConditions = value; }
    }

    [ShowInInspector]
    [PropertyOrder(7)]
    [TitleGroup("Progress Conditions")]
    [ShowIf("QuestPresent")]
    [AutoTable(typeof(VCondition), "Reward Prerequisites", NodeColors.BASIC_COLOR)]
    public List<VCondition> RewardConditions {
      get => Quest?.RewardConditions;
      set { if (Quest != null) Quest.RewardConditions = value; }
    }

    [ShowInInspector]
    [PropertyOrder(7)]
    [TitleGroup("Progress Conditions")]
    [ShowIf("ShowSkipFields")]
    [AutoTable(typeof(VCondition), "Skip Prerequisites", NodeColors.BASIC_COLOR)]
    public List<VCondition> SkipConditions {
      get => Quest?.SkipConditions;
      set { if (Quest != null) Quest.SkipConditions = value; }
    }

    // --- Triggers ---
    [ShowInInspector]
    [PropertyOrder(8)]
    [TitleGroup("Progress Triggers")]
    [ShowIf("QuestPresent")]
    [AutoTable(typeof(VTrigger), "On Quest Availability", NodeColors.BASIC_COLOR)]
    public List<VTrigger> AvailabilityTriggers {
      get => Quest?.AvailabilityTriggers;
      set { if (Quest != null) Quest.AvailabilityTriggers = value; }
    }

    [ShowInInspector]
    [PropertyOrder(9)]
    [TitleGroup("Progress Triggers")]
    [ShowIf("QuestPresent")]
    [AutoTable(typeof(VTrigger), "On Quest Start", NodeColors.BASIC_COLOR)]
    public List<VTrigger> StartTriggers {
      get => Quest?.StartTriggers;
      set { if (Quest != null) Quest.StartTriggers = value; }
    }

    [ShowInInspector]
    [PropertyOrder(10)]
    [TitleGroup("Progress Triggers")]
    [ShowIf("QuestPresent")]
    [AutoTable(typeof(VTrigger), "On Quest Completion", NodeColors.BASIC_COLOR)]
    public List<VTrigger> CompletionTriggers {
      get => Quest?.CompletionTriggers;
      set { if (Quest != null) Quest.CompletionTriggers = value; }
    }

    [ShowInInspector]
    [PropertyOrder(11)]
    [TitleGroup("Progress Triggers")]
    [ShowIf("QuestPresent")]
    [AutoTable(typeof(VTrigger), "On Reward Collection", NodeColors.BASIC_COLOR)]
    public List<VTrigger> RewardTriggers {
      get => Quest?.RewardTriggers;
      set { if (Quest != null) Quest.RewardTriggers = value; }
    }

    [ShowInInspector]
    [PropertyOrder(11)]
    [TitleGroup("Progress Triggers")]
    [ShowIf("ShowSkipFields")]
    [AutoTable(typeof(VTrigger), "On Quest Skipped", NodeColors.BASIC_COLOR)]
    public List<VTrigger> SkipTriggers {
      get => Quest?.SkipTriggers;
      set { if (Quest != null) Quest.SkipTriggers = value; }
    }

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void PostHandle(GraphEngine graphEngine) {
      QuestStartNode innerStart = Quest.FindNode<QuestStartNode>();
      graphEngine.RemoveNode(this);
      graphEngine.AddNode(innerStart);
      graphEngine.Continue();
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void MarkStarted() {
      progress = QuestProgress.Started;
    }

    public void MarkComplete() {
      progress = QuestProgress.Completed;
    }

    public void MarkSkipped() {
      if (!Required &&
           Missable &&
          !(Quest?.Progress == QuestProgress.Completed || Quest?.Progress == QuestProgress.RewardsCollected)) {
            
        Quest?.MarkSkipped();
        progress = QuestProgress.Skipped;
      }
    }

    private bool ShowSkipFields() {
      return Quest != null && !Required && Missable;
    }

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected override void OnEnable() {
      base.OnEnable();
      if (!Required && Missable) {
        ((QuestGraph)graph).RegisterOptionalObjective(this);
      }
    }

    public override bool IsNodeComplete() {
      foreach (NodePort port in Ports) {
        if (!port.IsConnected || port.GetConnections().Count == 0) {
          if (port.IsOutput && required) {
            return false;
          }
        }
      }

      return true;
    }
#if UNITY_EDITOR
    //-------------------------------------------------------------------------
    // Editor Stuff
    //-------------------------------------------------------------------------
    private bool QuestPresent() {
      if (Quest != null) {
        return true;
      }
      return false;
    }

    public void ChangeQuest(QuestGraph quest) {
      Quest = quest;
      OnQuestChange();
    }

    public void OnQuestChange() {
      #if UNITY_EDITOR
      Quest?.SetParent((QuestGraph)graph);
      Quest?.SetParentNode(this);

      if (string.IsNullOrEmpty(Quest?.Title) && !string.IsNullOrEmpty(title)) {
        Quest.Title = title;
      }

      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      EditorUtility.SetDirty(this);
      if (Quest != null) {
        EditorUtility.SetDirty(Quest);
      }
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
      #endif
    }

    public void OnTitleChanged() {
      #if UNITY_EDITOR
      name = string.IsNullOrWhiteSpace(Title) ? "Quest" : "Quest: " + Title;
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      #endif
    }

    [PropertySpace(10)]
    [Button("Create Subquest", ButtonSizes.Large)]
    [GUIColor(.25f, .27f, .44f)]
    [PropertyOrder(996)]
    [HideIf("QuestPresent")]
    public void CreateAndSaveQuest() {
      Quest = ScriptableObject.CreateInstance<QuestGraph>();
      NodeEditorWindow.Open(Quest);
      string path = AssetDatabase.GetAssetPath(graph);
      string folder = path.Substring(0, path.LastIndexOf('/'));
      Debug.Log(folder);

      if (!NodeEditorWindow.current.SaveAs(folder)) {
        DestroyImmediate(Quest);
        Quest = null;
      } else {
        OnQuestChange();
      }

      NodeEditorWindow.Open(graph);
    }

    [PropertySpace(10)]
    [Button("Open Subquest", ButtonSizes.Large)]
    [GUIColor(.25f, .27f, .44f)]
    [PropertyOrder(996)]
    [ShowIf("QuestPresent")]
    public void Open() {
      NodeEditorWindow.Open(Quest);
    }

    [PropertySpace(10)]
    [HorizontalGroup("Removal")]
    [Button("Detach Subquest", ButtonSizes.Medium)]
    [GUIColor(.5f, .5f, .5f)]
    [PropertyOrder(997)]
    [ShowIf("QuestPresent")]
    public void RemoveGraph() {
      if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to detach this quest?", "Remove", "Cancel")) {
        Quest = null;
        OnQuestChange();
      }
    }

    [PropertySpace(10)]
    [HorizontalGroup("Removal")]
    [Button("Destroy Subquest", ButtonSizes.Medium)]
    [GUIColor(.5f, .2f, .2f)]
    [PropertyOrder(998)]
    [ShowIf("QuestPresent")]
    public void DestroyGraph() {
      if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to delete this quest? This cannot be undone!", "Delete", "Cancel")) {
        string path = AssetDatabase.GetAssetPath(Quest);
        AssetDatabase.DeleteAsset(path);
        Quest = null;
      }
    }

    public void RewardsChanged() {
      Debug.Log("Quest Node Reward Change");
      if (Quest != null) {
        Quest.Rewards = Rewards;
      }
    }
#endif
  }
}
