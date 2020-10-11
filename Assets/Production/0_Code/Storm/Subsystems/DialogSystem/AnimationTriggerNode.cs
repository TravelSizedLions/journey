using XNode;

using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;
using Storm.Characters.Player;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Dialog/Animation/Animation Trigger")]
  public class AnimationTriggerNode : DialogNode {


    #region Fields
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The Animator controller to change.
    /// </summary>
    [Tooltip("The Animator controller to change.")]
    public Animator Animator;

    /// <summary>
    /// The name of the trigger to set."
    /// </summary>
    [Tooltip("The name of the trigger to set.")]
    [ValueDropdown("Triggers")]
    public string Trigger;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    #endregion

    #region XNode API
    //---------------------------------------------------
    // XNode API
    //---------------------------------------------------

    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) => null;

    #endregion

    #region Dialog Node API
    public override void Handle() {
      if (Animator == null) {
        if (player == null) {
          player = GameManager.Player;
        }
        
        Animator = player.GetComponent<Animator>();
      }

      Animator.SetTrigger(Trigger);
    }

    #endregion

    #region Odin Inspector
    private IEnumerable Triggers() {
      List<string> trigs = new List<string>();
      if (Animator == null) {
        Animator = Resources.FindObjectsOfTypeAll<PlayerCharacter>()[0].GetComponent<Animator>();
      }
      
      foreach (AnimatorControllerParameter param in Animator.parameters) {
        trigs.Add(param.name);
      }

      if (Trigger == null) {
        Trigger = "-- select a trigger --";
      }

      return trigs;
    }
    #endregion
  }
}