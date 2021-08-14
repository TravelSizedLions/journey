using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class EditorLockToParentTransform : MonoBehaviour {
    [OnInspectorGUI]
    private void LockToParentPosition() {
      transform.localPosition = Vector3.zero;
    }
  }
}