using System;
using UnityEngine;


namespace Storm.Subsystems.Transitions {
  [Serializable]
  public class TransitionEffect : MonoBehaviour {
    
    /// <summary>
    /// The name of the effect.
    /// </summary>
    [Tooltip("The name of the effect.")]
    public string Name;

    /// <summary>
    /// The animator associated with this effect.
    /// </summary>
    public Animator Animator;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="trigger"></param>
    public void SetTrigger(string trigger) {
      Animator.SetTrigger(trigger);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetBool(string name, bool value) {
      Animator.SetBool(name, value);
    }
  }
}