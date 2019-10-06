using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Flexible {
    public class SelfDestructing : MonoBehaviour
    {
        public void SelfDestruct() {
            Destroy(this.gameObject);
        }
    }
}


