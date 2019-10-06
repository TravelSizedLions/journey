using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Extensions;

namespace Storm.ResetSystem {
        
    public class ResetManager : Singleton<ResetManager> {
        private List<Resetting> resetListenters;

        public void Reset() {
            if (resetListenters == null) {
                resetListenters = new List<Resetting>(GameObject.FindObjectsOfType<Resetting>());
            }

            foreach (var r in resetListenters) {
                r.Reset();
            }
        }
    }
}