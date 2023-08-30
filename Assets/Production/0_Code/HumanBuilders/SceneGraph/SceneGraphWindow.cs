using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TSL.SceneGraphSystem {
  public class SceneGraphWindow : EditorWindow {

    private const string  SCENE_GRAPH_PATH = "Assets/Production/Resources/scene-graph.asset";
    private SceneGraphView graphView;
    private InspectorView inspectorView;

    [MenuItem("TSL/Scene Graph")]
    public static void ShowWindow() {
      SceneGraphWindow window = GetWindow<SceneGraphWindow>();
      window.titleContent = new GUIContent("Scene Graph");
    }

    public void CreateGUI() {
      // Each editor window contains a root VisualElement object
      VisualElement root = rootVisualElement;

      // Instantiate UXML
      VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Production/0_Code/HumanBuilders/Editor/SceneGraph/SceneGraphWindow.uxml");
      tree.CloneTree(root);

      StyleSheet styles = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Production/0_Code/HumanBuilders/Editor/SceneGraph/SceneGraphWindow.uss");
      root.styleSheets.Add(styles);

      graphView = root.Q<SceneGraphView>();
      inspectorView = root.Q<InspectorView>();

      BuildGraph();
    }

    private void BuildGraph() {
      SceneGraph graph = AssetDatabase.LoadAssetAtPath<SceneGraph>(SCENE_GRAPH_PATH);
      if (graph) {
        graphView.PopulateView(graph);
      } else {
        graph = ScriptableObject.CreateInstance<SceneGraph>();
        AssetDatabase.CreateAsset(graph, SCENE_GRAPH_PATH);
        AssetDatabase.SaveAssets();
        graph.Construct();
        AssetDatabase.SaveAssets();
        graphView.PopulateView(graph);
      }
    }
  }
}