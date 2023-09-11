#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;

namespace TSL.Subsystems.WorldView {
  [CustomNodeEditor(typeof(SceneNode))]
  public class SceneNodeEditor : NodeEditor {
    public override void AddContextMenuItems(GenericMenu menu) {
      if (Selection.objects.Length == 1 && Selection.activeObject is XNode.Node) {
        XNode.Node node = Selection.activeObject as XNode.Node;
        menu.AddCustomContextMenuItems(node);
      }
    }
  }
}
#endif