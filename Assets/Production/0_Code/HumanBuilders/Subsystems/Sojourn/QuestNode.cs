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
  public class QuestNode : AutoNode, ISojournNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    
    [Input(connectionType = ConnectionType.Multiple)]
    [PropertyOrder(0)]
    public EmptyConnection Input;

    [Output(connectionType = ConnectionType.Multiple)]
    [PropertyOrder(5)]
    [PropertySpace(10)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [OnValueChanged("OnQuestChange")]
    public QuestAsset Quest;

    private QuestAsset prevAttached;

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      Debug.Log("QuestNode");
      base.Handle(graphEngine);
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

    public void OnQuestChange() {
      Quest?.SetParent((QuestAsset)graph);
      prevAttached?.SetParent(null);

      prevAttached = Quest;
    }

    [PropertySpace(10)]
    [Button("Create Subquest", ButtonSizes.Large)]
    [GUIColor(.25f, .27f, .44f)]
    [PropertyOrder(2)]
    [HideIf("QuestPresent")]
    public void CreateAndSaveQuest() {
      Quest = ScriptableObject.CreateInstance<QuestAsset>();
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
    [PropertyOrder(3)]
    [ShowIf("QuestPresent")]
    public void Open() {
      NodeEditorWindow.Open(Quest);
    }

    [PropertySpace(10)]
    [HorizontalGroup("Removal")]
    [Button("Detach Subquest", ButtonSizes.Medium)]
    [GUIColor(.5f, .5f, .5f)]
    [PropertyOrder(4)]
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
    [PropertyOrder(4)]
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
