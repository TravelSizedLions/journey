#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TSL.SceneGraphSystem {
  public class SceneGraphWindow : EditorWindow {

    private SceneGraphView graphView;

    [MenuItem("TSL/Scene Graph")]
    public static void ShowWindow() {
      SceneGraphWindow window = GetWindow<SceneGraphWindow>();
      window.titleContent = new GUIContent("Scene Graph");
    }

    public void CreateGUI() {
      // Each editor window contains a root VisualElement object
      VisualElement root = rootVisualElement;

      // Instantiate UXML
      VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{SceneGraphConsts.ASSETS_FOLDER}/SceneGraphWindow.uxml");
      tree.CloneTree(root);

      StyleSheet styles = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{SceneGraphConsts.ASSETS_FOLDER}/SceneGraphWindow.uss");
      root.styleSheets.Add(styles);

      graphView = root.Q<SceneGraphView>();

      BuildGraph();
    }

    private void BuildGraph() {
      SceneGraph graph = AssetDatabase.LoadAssetAtPath<SceneGraph>(SceneGraphConsts.GRAPH_PATH);
      if (graph) {
        graphView.PopulateView(graph);
      } else {
        graph = ScriptableObject.CreateInstance<SceneGraph>();
        AssetDatabase.CreateAsset(graph, SceneGraphConsts.GRAPH_PATH);
        AssetDatabase.SaveAssets();
        graph.Construct();
        AssetDatabase.SaveAssets();
        graphView.PopulateView(graph);
      }
    }
  }
}
#endif