using System.Collections;
using System.Collections.Generic;
using Storm.Extensions;
using UnityEngine;

namespace Storm.ResetSystem {

  public class ResetManager : Singleton<ResetManager> {

    public void Reset() {
      foreach (var r in GameObject.FindObjectsOfType<Resetting>()) {
        r.Reset();
      }
    }
  }
}