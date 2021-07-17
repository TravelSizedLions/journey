using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace HumanBuilders {

  [CreateNodeMenu("Quest")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class QuestNode : JourneyNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [Input(connectionType = ConnectionType.Multiple)]
    [PropertyOrder(0)]
    public EmptyConnection Input;

    [Output(connectionType = ConnectionType.Multiple)]
    [PropertyOrder(999)]
    [PropertySpace(10)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [OnValueChanged("OnQuestChange")]
    [PropertyOrder(1)]
    public QuestGraph Quest;
    private QuestGraph prevAttached;

    [PropertyOrder(3)]
    [ShowInInspector]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Rewards")]
    [AutoTable(typeof(ITriggerable), "Quest Rewards", NodeColors.BASIC_COLOR)]
    public AutoTable<ITriggerable> Rewards {
      get => Quest?.Rewards;
      set { if (Quest != null) Quest.Rewards = value; }
    }

    [PropertyOrder(4)]
    [ShowInInspector]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(ITriggerable), "World Triggers On Availability", NodeColors.BASIC_COLOR)]
    public AutoTable<ITriggerable> AvailabilityTriggers {
      get => Quest?.AvailabilityTriggers;
      set { if (Quest != null) Quest.AvailabilityTriggers = value; }
    }

    [ShowInInspector]
    [PropertyOrder(5)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(ITriggerable), "World Triggers On Start", NodeColors.BASIC_COLOR)]
    public AutoTable<ITriggerable> StartTriggers {
      get => Quest?.StartTriggers;
      set { if (Quest != null) Quest.StartTriggers = value; }
    }

    [ShowInInspector]
    [PropertyOrder(6)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(ITriggerable), "World Triggers on Quest Completion", NodeColors.BASIC_COLOR)]
    public AutoTable<ITriggerable> CompletionTriggers {
      get => Quest?.CompletionTriggers;
      set { if (Quest != null) Quest.CompletionTriggers = value; }
    }

    [ShowInInspector]
    [PropertyOrder(7)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(ITriggerable), "World Triggers on Reward Collection", NodeColors.BASIC_COLOR)]
    public AutoTable<ITriggerable> RewardTriggers {
      get => Quest?.RewardTriggers;
      set { if (Quest != null) Quest.RewardTriggers = value; }
    }

    [ShowInInspector]
    [PropertyOrder(8)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [AutoTable(typeof(ICondition), "Availability Conditions", NodeColors.BASIC_COLOR)]
    public AutoTable<ICondition> AvailabilityConditions {
      get => Quest?.AvailabilityConditions;
      set { if (Quest != null) Quest.AvailabilityConditions = value; }
    }

    [ShowInInspector]
    [PropertyOrder(9)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [AutoTable(typeof(ICondition), "Start Conditions", NodeColors.BASIC_COLOR)]
    public AutoTable<ICondition> StartConditions {
      get => Quest?.StartConditions;
      set { if (Quest != null) Quest.StartConditions = value; }
    }

    [ShowInInspector]
    [PropertyOrder(10)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [AutoTable(typeof(ICondition), "Extra Completion Conditions", NodeColors.BASIC_COLOR)]
    public AutoTable<ICondition> CompletionConditions {
      get => Quest?.CompletionConditions;
      set { if (Quest != null) Quest.CompletionConditions = value; }
    }

    [ShowInInspector]
    [PropertyOrder(11)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [AutoTable(typeof(ICondition), "Extra Reward Conditions", NodeColors.BASIC_COLOR)]
    public AutoTable<ICondition> RewardConditions {
      get => Quest?.RewardConditions;
      set { if (Quest != null) Quest.RewardConditions = value; }
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
    public void Start() {
      progress = QuestProgress.Started;
    }

    public void MarkComplete() {
      progress = QuestProgress.Completed;
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
      Quest?.SetParent((QuestGraph)graph);
      Quest?.SetParentNode(this);
      prevAttached?.SetParent(null);
      prevAttached?.SetParentNode(null);

      prevAttached = Quest;
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
