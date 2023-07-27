using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class Skybox : MonoBehaviour {
    
    [OnInspectorGUI]
    private void FindCamera() {
      Canvas canv = GetComponent<Canvas>();
      if (canv.worldCamera == null) {
        TargettingCamera cam = FindObjectOfType<TargettingCamera>();
        canv.worldCamera = cam.GetComponent<Camera>();
      }
    }

    private void Awake() {
      FindCamera();
    }
  }
}