#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace TSL.SceneGraphSystem {
  public class InspectorView : VisualElement {
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> {}
    public InspectorView() {

    }
  }
}
#endif