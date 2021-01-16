using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public enum TriggerType {
    Automatic,
    Collider
  }

  /// <summary>
  /// A class that triggers a cutscene automatically on loading a new scene.
  /// </summary>
  public class CutsceneTrigger : Interactible {

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

    /// <summary>
    /// What causes the cutscene to start playing? Does it start automaticially,
    /// or does the player need to walk into a certain area?
    /// </summary>
    [Tooltip("When to trigger the cutscene.\nAutomatic - When the scene loads.\nCollider - When the player moves into the trigger collider attached to this GameObject.")]
    public TriggerType TriggerType = TriggerType.Automatic;

    /// <summary>
    /// Whether or not the cutscene has already played.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private bool fired = false;

    private void Start() {
      if (TriggerType == TriggerType.Automatic) {
        new UnityTask(_Trigger());
      }
    }

    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player") && !fired && TriggerType == TriggerType.Collider) {
        new UnityTask(_Trigger());
        fired = true;
      }
    }

    private IEnumerator _Trigger() {
      if (Delay > 0) {
        yield return new WaitForSeconds(Delay);
      }

      if (Cutscene != null) {
        GameManager.Player.Interact(this);
      }
    }


    public override void OnInteract() {
      if (!interacting) {
        interacting = true;
        DialogManager.StartDialog(Cutscene);
      } else {
        DialogManager.ContinueDialog();
        if (DialogManager.IsDialogFinished()) {
          interacting = false;
        }
      }
      
    }
  }

}