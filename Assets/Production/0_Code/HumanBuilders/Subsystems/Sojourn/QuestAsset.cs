#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor.Graphs;
using System;

namespace HumanBuilders {
  [CreateAssetMenu(fileName="New Quest", menuName="Sojourn/Quest")]
  [RequireNode(typeof(QuestStartNode), typeof(QuestEndNode))]
  public class QuestAsset : AutoGraphAsset {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private QuestAsset parentQuest;

    //-------------------------------------------------------------------------
    // Public API
    //-------------------------------------------------------------------------
    public void SetParent(QuestAsset parent) {
      parentQuest = parent;
    }

    public QuestAsset GetParent() {
      return parentQuest;
    }

#if UNITY_EDITOR
    //-------------------------------------------------------------------------
    // Editor Stuff
    //-------------------------------------------------------------------------
    [ContextMenu("To Parent Quest")]
    public void Exit() {
      if (parentQuest != null) {
        NodeEditorWindow.Open(parentQuest);
      }
    }
#endif
  }
}