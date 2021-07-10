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

    [SerializeField]
    [PropertyOrder(3)]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [ConditionTable("Availability Conditions", 0.290f, 0.298f, 0.509f)]
    private List<ConditionTableEntry> AvailabilityConditions = null;

    [SerializeField]
    [Space(10)]
    [PropertyOrder(4)]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [ConditionTable("Start Conditions", 0.290f, 0.298f, 0.509f)]
    private List<ConditionTableEntry> StartConditions = null;

    [SerializeField]
    [Space(10)]
    [PropertyOrder(5)]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [ConditionTable("Extra Completion Conditions", 0.290f, 0.298f, 0.509f)]
    public List<ConditionTableEntry> CompletionConditions = null;

    [SerializeField]
    [Space(10)]
    [PropertyOrder(6)]
    [FoldoutGroup("Extra Quest Conditions", false)]
    [ConditionTable("Extra Reward Conditions", 0.290f, 0.298f, 0.509f)]
    public List<ConditionTableEntry> RewardConditions = null;



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
