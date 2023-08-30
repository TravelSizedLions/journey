#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using TSL.Editor.SceneUtilities;

namespace TSL.SceneGraphSystem {
  public class SceneGraphView : GraphView {
    public new class UxmlFactory : UxmlFactory<SceneGraphView, GraphView.UxmlTraits> { }

    private SceneGraph graph;

    public SceneGraphView() {
      Insert(0, new GridBackground());

      this.AddManipulator(new ContentZoomer());
      this.AddManipulator(new ContentDragger());
      this.AddManipulator(new SelectionDragger());
      this.AddManipulator(new RectangleSelector());

      StyleSheet styles = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{SceneGraphConsts.ASSETS_FOLDER}/SceneGraphWindow.uss");
      styleSheets.Add(styles);
    }

    public void PopulateView(SceneGraph graph) {
      this.graph = graph;

      graphViewChanged -= OnGraphViewChanged;
      DeleteElements(graphElements);
      graphViewChanged += OnGraphViewChanged;

      graph.Nodes.ForEach(node => CreateNodeView(node));
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
      if (graphViewChange.elementsToRemove != null) {
        graphViewChange.elementsToRemove.ForEach(elem => {
          SceneNodeView nodeView = elem as SceneNodeView;
          if (nodeView != null) {
            graph.DeleteNode(nodeView.node);
          }
        });
      }
      return graphViewChange;
    }

    private void CreateNodeView(SceneNode node) {
      SceneNodeView nodeView = new SceneNodeView(node);
      AddElement(nodeView);
    }

    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
      // TypeCache.GetTypesDerivedFrom<Type>(); <-- in case it's ever needed.
  
      evt.menu.AppendAction("Re-analyze Scenes", a => {
        EdgeBuilder.BuildEdges(graph);
      });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
      return ports.ToList();
    }
  }

}
#endif