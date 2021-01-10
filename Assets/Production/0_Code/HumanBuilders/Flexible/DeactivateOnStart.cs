
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// This class disables a script on start.
  /// </summary>
  /// <remarks>
  /// Very, very unfortunately, there's not an equivalent to Awake/Start that
  /// fires even when an object starts off inactive. To mimic the behavior, this
  /// attach this script to anything that you'd like to start off disabled but
  /// still have the Awake/Start methods fire for the object. 
  /// </remarks>
  public class DeactivateOnStart : MonoBehaviour {

    private void Start() {
      gameObject.SetActive(false);
    }

  }
}