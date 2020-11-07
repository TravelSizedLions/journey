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
  }
}