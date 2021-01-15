using System.Collections;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A class that triggers a cutscene automatically on loading a new scene.
  /// </summary>
  public class CutsceneTrigger : MonoBehaviour {

    /// <summary>
    /// The cutscene to trigger.
    /// </summary>
    [Tooltip("The cutscene to trigger.")]
    public AutoGraph Cutscene;

    /// <summary>
    /// The number of seconds to delay before starting the cutscene.
    /// </summary>
    [Tooltip("The number of seconds to delay before starting the cutscene.")]
    public float Delay;

    private void Awake() {
      new UnityTask(_Trigger());
    }

    private IEnumerator _Trigger() {
      if (Delay > 0) {
        yield return new WaitForSeconds(Delay);
      }

      if (Cutscene != null) {
        DialogManager.StartDialog(Cutscene);
      }
    }

  }

}