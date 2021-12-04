using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif

namespace HumanBuilders {

  /// <summary>
  /// A node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("")]
  [Obsolete("Use AnimationParamNode instead.")]
  public class AnimationTriggerNode : AutoNode {

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
    [ValueDropdown(nameof(Triggers))]
    public string Trigger;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

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

    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
    private IEnumerable Triggers() {
      List<string> trigs = new List<string>();
      
      #if UNITY_EDITOR
      if (Animator == null) {
        Animator = Resources.FindObjectsOfTypeAll<PlayerCharacter>()[0].GetComponent<Animator>();
      }

      AnimatorController editorController = (AnimatorController)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(Animator.runtimeAnimatorController), typeof(AnimatorController));
      if (editorController != null) {
        foreach (AnimatorControllerParameter param in editorController.parameters) {
          if (param.type == AnimatorControllerParameterType.Trigger) {
            trigs.Add(param.name);
          }
        }
      }

      if (Trigger == null) {
        Trigger = "-- select a trigger --";
      }
      #endif
      
      return trigs;
    }
  }
}