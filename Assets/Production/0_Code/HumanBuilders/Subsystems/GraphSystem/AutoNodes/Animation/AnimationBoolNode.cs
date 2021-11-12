using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System;

namespace HumanBuilders {

  /// <summary>
  /// A node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("")]
  [Obsolete("Use AnimationParamNode instead.")]
  public class AnimationBoolNode : AutoNode {
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The Animator controller to change. If left blank, the default is the
    /// player character's animator.
    /// </summary>
    [Tooltip("The Animator controller to change.")]
    [SerializeField]
    [OnValueChanged(nameof(BoolParams))]
    public Animator Animator;

    /// <summary>
    /// The name of the parameter to set.
    /// </summary>
    [Tooltip("The name of the parameter to set.")]
    [SerializeField]
    [ValueDropdown(nameof(BoolParams))]
    public string Parameter;

    /// <summary>
    /// The boolean to set in the Animator.
    /// </summary>
    [Tooltip("The boolean to set.")]
    [SerializeField]
    public bool Value;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;


    //---------------------------------------------------
    // Unity API
    //---------------------------------------------------
    private void Awake() {

    }

    //---------------------------------------------------
    // Auto Node API
    //---------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (Animator == null) {
        if (player == null) {
          player = GameManager.Player;
        }

        Animator = player.GetComponent<Animator>();
      }

      Animator.SetBool(Parameter, Value);
    }

    //-------------------------------------------------------------------------
    // Odin Inspector Stuff
    //-------------------------------------------------------------------------
    #if UNITY_EDITOR
    private IEnumerable BoolParams() {
      List<string> parms = new List<string>();

      if (Animator != null) {
        foreach (var param in Animator.parameters) {
          if (param.type == AnimatorControllerParameterType.Bool) {
            parms.Add(param.name);
          }
        }
      }

      if (Parameter == null) {
        Parameter = "-- select a parameter --";
      }

      return parms;
    }
    #endif
  }
}