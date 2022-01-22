using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HumanBuilders {
  public class PlaceAtRoot : MonoBehaviour {
    private void Awake() {
      transform.parent = null;
    }
  }

}