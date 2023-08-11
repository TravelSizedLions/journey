using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class Skybox : MonoBehaviour {
    Canvas canv;

    [OnInspectorGUI]
    private Camera FindCamera() {
      Canvas canv = GetComponent<Canvas>();
      if (canv.worldCamera == null) {
        TargettingCamera cam = FindObjectOfType<TargettingCamera>();
        if (cam != null) {
          return cam.GetComponent<Camera>();
        }
        
        Debug.Log("Couldn't find Targetting Camera");
      }

      return null;
    }

    private void Awake() {
      canv = GetComponent<Canvas>();
      FindCamera();
    }

    private void Update() {
      if (canv.worldCamera == null) {
        Debug.LogWarning("lost camera on skybox. searching...");
        canv.worldCamera = FindCamera();
      }
    }
  }
}