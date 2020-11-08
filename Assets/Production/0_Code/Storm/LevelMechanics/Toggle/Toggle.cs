using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Flexible.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Storm.LevelMechanics {

  /// <summary>
  /// Something that the player can switch on or off.
  /// </summary>
  public class Toggle : Interactible {

    /// <summary>
    /// Whether or not this object is toggled on or off.
    /// </summary>
    public bool IsOn { get { return isOn; } }

    /// <summary>
    /// Whether or not this object is toggled on or off.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not this object is toggled on or off.")]
    [PropertySpace(SpaceAfter = 10)]
    private bool isOn;

    /// <summary>
    /// The events that should fire when the object is toggled on.
    /// </summary>
    [FoldoutGroup("Toggle Behavior")]
    [Tooltip("The events that should fire when the object is toggled on.")]
    public UnityEvent ToggleOn;

    /// <summary>
    /// The events that should fire when the object is toggled off.
    /// </summary>
    [FoldoutGroup("Toggle Behavior")]
    [Tooltip("The events that should fire when the object is toggled off.")]
    public UnityEvent ToggleOff;

    /// <summary>
    /// The animator for on/off visual behavior.
    /// </summary>
    private Animator animator;


    protected new void Awake() {
      base.Awake();
      animator = GetComponent<Animator>();
    }


    /// <summary>
    /// Toggles the object on/off. Then, perform events based on whether the
    /// object is toggled on/off.
    /// </summary>
    public override void OnInteract() {
      isOn = !isOn;

      if (isOn && ToggleOn.GetPersistentEventCount() > 0) {
        ToggleOn.Invoke();
      } else if (!isOn && ToggleOff.GetPersistentEventCount() > 0) {
        ToggleOff.Invoke();
      }

      if (animator != null) {
        animator.SetBool("on", isOn);
      }
    }

    /// <summary>
    /// The conditions under which the indicator for this object should be
    /// shown. This condition will be checked if this is the closest
    /// interactive object to the player.
    /// </summary>
    public override bool ShouldShowIndicator() {
      return player.CarriedItem == null;
    }

  }
}