#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using HumanBuilders;
using XNodeEditor;
using System;
using XNode;

namespace HumanBuilders.Editor {
  [CustomNodeGraphEditor(typeof(QuestAsset))]
  public class QuestGraphEditor : NodeGraphEditor {

    public override void OnCreate() {
      Debug.Log("Adding extras");
      base.OnCreate();
      ((QuestAsset)target)?.OnCreate();
    }

    public override string GetNodeMenuName(Type type) {
      if (typeof(SojournNode).IsAssignableFrom(type)) {
          return base.GetNodeMenuName(type);
      } else return null;
    }

    public override bool CanRemove(Node node) {
      Type t = node.GetType();
      return base.CanRemove(node) && 
             !typeof(QuestEndNode).IsAssignableFrom(t) &&
             !typeof(QuestStartNode).IsAssignableFrom(t);
    }
  }
}

#endif