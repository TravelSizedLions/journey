using System.Collections.Generic;
using HumanBuilders;
using TSL.Editor.SceneUtilities;
using UnityEditor.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace TSL.Subsystems.WorldView {
  public class WorldViewWindow : NodeEditorWindow {
    /// <summary> Create editor window </summary>
    [MenuItem("TSL/WorldView")]
    public new static NodeEditorWindow Init() {
      NodeEditorWindow w = NodeEditorWindow.Init();
      w.titleContent = new GUIContent("World View");
      WorldViewGraph graph = AssetDatabase.LoadAssetAtPath<WorldViewGraph>(WorldViewSettings.GRAPH_PATH);
      if (graph) {
        Open(graph);
      } else {
        graph = ScriptableObject.CreateInstance<WorldViewGraph>();
        AssetDatabase.CreateAsset(graph, WorldViewSettings.GRAPH_PATH);
        AssetDatabase.SaveAssets();
        graph.Rebuild();
        AssetDatabase.SaveAssets();
        Open(graph);
      }
      return w;
    }


    public new static NodeEditorWindow Open(XNode.NodeGraph graph) {
      if (!graph) return null;

      NodeEditorWindow w = GetWindow(typeof(NodeEditorWindow), false, "World View", true) as NodeEditorWindow;
      w.wantsMouseMove = true;
      w.graph = graph;
      return w;
    }
  }
}