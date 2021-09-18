
using UnityEngine;

namespace HumanBuilders {
  public class DDOL: MonoBehaviour {
    public void Awake() {
      DontDestroyOnLoad(this);
    }
  }
}