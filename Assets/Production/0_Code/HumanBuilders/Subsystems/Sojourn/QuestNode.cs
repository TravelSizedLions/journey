
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace HumanBuilders {

  [CreateNodeMenu("Quest")]
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  public class QuestNode : SojournNode {
    
    [Input(connectionType = ConnectionType.Multiple)]
    [PropertyOrder(0)]
    public EmptyConnection Input;

    [Output(connectionType = ConnectionType.Multiple)]
    [PropertyOrder(3)]
    [PropertySpace(10)]
    public EmptyConnection Output;

    private bool questCreated = false;

    private QuestAsset quest;

#if UNITY_EDITOR
    [PropertySpace(10)]
    [Button("Create Graph", ButtonSizes.Large)]
    [GUIColor(.25f, .27f, .44f)]
    [PropertyOrder(2)]
    [HideIf("questCreated")]
    public void CreateGraph() {
      questCreated = true;
      quest = ScriptableObject.CreateInstance<QuestAsset>();
      quest.SetParent((QuestAsset)graph);
    }

    [PropertySpace(10)]
    [Button("Open Graph", ButtonSizes.Large)]
    [GUIColor(.25f, .27f, .44f)]
    [PropertyOrder(2)]
    [ShowIf("questCreated")]
    public void Open() {
      NodeEditorWindow.Open(quest);
    }

    [PropertySpace(10)]
    [Button("Remove Graph", ButtonSizes.Small)]
    [GUIColor(.5f, .2f, .2f)]
    [PropertyOrder(2)]
    [ShowIf("questCreated")]
    public void RemoveGraph() {
      Destroy(quest);
      quest = null;
      questCreated = false;
    }
#endif

  }
}