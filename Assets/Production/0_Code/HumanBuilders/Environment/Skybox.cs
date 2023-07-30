using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class Skybox : MonoBehaviour {
    private Camera cam;

    [OnInspectorGUI]
    private Camera FindCamera() {
      Canvas canv = GetComponent<Canvas>();
      if (canv.worldCamera == null) {
        TargettingCamera cam = FindObjectOfType<TargettingCamera>();
        if (cam != null) {
          canv.worldCamera = cam.GetComponent<Camera>();
          return canv.worldCamera;
        }
        
        Debug.Log("Couldn't find Targetting Camera");
      }

      return null;
    }

    private void Awake() {
      FindCamera();
    }

    private void Update() {
      if (cam == null) {
        Debug.LogWarning("lost camera on skybox. searching...");
        cam = FindCamera();
      }
    }
  }
}