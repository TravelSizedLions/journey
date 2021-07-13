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

    // TODO: Make two-way binding for the fields below between this and QuestStartNode/QuestEndNode
    [ShowInInspector]
    [PropertyOrder(2)]
    public override bool Required { 
      get => required;
      set => required = value;
    }

    [PropertyOrder(3)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Rewards")]
    [AutoTable(typeof(Reward), "Quest Rewards", NodeColors.BASIC_COLOR)]
    public AutoTable<Reward> Rewards = null;

    [PropertyOrder(4)]
    [SerializeField]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(WorldTrigger), "World Triggers On Availability", NodeColors.BASIC_COLOR)]
    private AutoTable<WorldTrigger> AvailabilityTriggers = null;

    [Space(10)]
    [PropertyOrder(5)]
    [SerializeField]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(WorldTrigger), "World Triggers On Start", NodeColors.BASIC_COLOR)]
    private AutoTable<WorldTrigger> StartTriggers = null;

    [Space(10)]
    [PropertyOrder(6)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(WorldTrigger), "World Triggers on Quest Completion", NodeColors.BASIC_COLOR)]
    public AutoTable<WorldTrigger> CompletionTriggers = null;

    [Space(10)]
    [PropertyOrder(7)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(WorldTrigger), "World Triggers on Reward Collection", NodeColors.BASIC_COLOR)]
    public AutoTable<WorldTrigger> RewardTriggers = null;

    [SerializeField]
    [PropertyOrder(8)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [AutoTable(typeof(ICondition), "Availability Conditions", NodeColors.BASIC_COLOR)]
    private AutoTable<ICondition> AvailabilityConditions = null;

    [SerializeField]
    [Space(10)]
    [PropertyOrder(9)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [AutoTable(typeof(ICondition), "Start Conditions", NodeColors.BASIC_COLOR)]
    private AutoTable<ICondition> StartConditions = null;

    [SerializeField]
    [Space(10)]
    [PropertyOrder(10)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [AutoTable(typeof(ICondition), "Extra Completion Conditions", NodeColors.BASIC_COLOR)]
    public AutoTable<ICondition> CompletionConditions = null;

    [SerializeField]
    [Space(10)]
    [PropertyOrder(11)]
    [ShowIf("QuestPresent")]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [AutoTable(typeof(ICondition), "Extra Reward Conditions", NodeColors.BASIC_COLOR)]
    public AutoTable<ICondition> RewardConditions = null;



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
#endif
  }
}
