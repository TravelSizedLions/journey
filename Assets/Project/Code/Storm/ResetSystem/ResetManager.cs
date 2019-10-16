using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Extensions;

namespace Storm.ResetSystem {
        
    public class ResetManager : Singleton<ResetManager> {

        public void Reset() {
            foreach (var r in GameObject.FindObjectsOfType<Resetting>()) {
                r.Reset();
            }
        }
    }
}