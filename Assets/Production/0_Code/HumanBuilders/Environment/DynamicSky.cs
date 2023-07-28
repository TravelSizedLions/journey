using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  public class DynamicSky : Singleton<DynamicSky> {

    private void OnDestroy() {
      var children = new List<GameObject>();

      foreach (var transform in GetComponentsInChildren<Transform>(true)) {
        children.Add(transform.gameObject);
      }

      children.ForEach(child => Destroy(child));
    }
  }
}