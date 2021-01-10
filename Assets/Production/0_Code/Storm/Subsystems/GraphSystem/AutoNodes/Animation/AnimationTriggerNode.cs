using XNode;

using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;


namespace HumanBuilders {

  /// <summary>
  /// A node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Animation Trigger")]
  public class AnimationTriggerNode : AutoNode {

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

    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
  
    public override void Handle(GraphEngine graphEngine) {
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
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
    
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