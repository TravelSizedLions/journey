#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using HumanBuilders;
using XNodeEditor;
using System;
using XNode;

namespace HumanBuilders.Editor {
  [CustomNodeGraphEditor(typeof(AutoGraphAsset))]
  public class AutoGraphEditor : NodeGraphEditor {
    public override string GetNodeMenuName(Type type) {
      if (typeof(IJourneyNode).IsAssignableFrom(type)) {
        return null;
      } else return base.GetNodeMenuName(type);
    }

    public override bool CanRemove(Node node) {
      Type t = node.GetType();
      return base.CanRemove(node) && 
             !typeof(QuestEndNode).IsAssignableFrom(t) &&
             !typeof(QuestStartNode).IsAssignableFrom(t);
    }

    public override void OnDropObjects(UnityEngine.Object[] objects) {
      float offset = 100;
      for (int i = 0; i < objects.Length; i++) {
        UnityEngine.Object obj = objects[i];
        if (obj.GetType() == typeof(QuestGraph)) {
          QuestGraph draggedQuest = (QuestGraph)obj; 
          QuestGraph graph = (QuestGraph)target;
          QuestNode closest = null;
          float minDist = float.PositiveInfinity;

          foreach (Node n in graph.nodes) {
            if (n is QuestNode questNode) {
              Vector2 dist = NodeEditorWindow.current.MousePosition - questNode.position;
              Debug.Log(dist.magnitude);

              if (questNode == null || minDist > dist.magnitude) {
                Debug.Log("Set");
                minDist = dist.magnitude;
                closest = questNode;
              }
            }
          }
          
          if (closest == null) {
            QuestNode node = (QuestNode)CreateNode(typeof(QuestNode), new Vector2(i*offset, i*offset));
            node.Quest = draggedQuest;
            node.Quest.SetParent((QuestGraph)node.graph);
          } else {
            closest.Quest = draggedQuest;
            draggedQuest.SetParent(graph);
          }
        }
      }
    }
  }
}

#endif