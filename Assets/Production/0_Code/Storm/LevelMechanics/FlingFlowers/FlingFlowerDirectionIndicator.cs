using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace Storm.LevelMechanics {
  /// <summary>
  /// Locks the direction indicators on the fling flower to the center of the parent.
  /// </summary>
  public class FlingFlowerDirectionIndicator : MonoBehaviour {

    [OnInspectorGUI]
    private void LockToParentPosition() {
      transform.localPosition = Vector3.zero;
    }

    private void OnDrawGizmos() {
      // Draw a ray to show where the indicator is actually pointing.
      if (ColorUtility.TryParseHtmlString("#42f58d44", out Color color)) {
        Gizmos.color = color;
      }

      float angle = Mathf.Deg2Rad*transform.localEulerAngles.z;
      Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

      Gizmos.DrawRay(transform.position, direction*400);
    }  
  }
}