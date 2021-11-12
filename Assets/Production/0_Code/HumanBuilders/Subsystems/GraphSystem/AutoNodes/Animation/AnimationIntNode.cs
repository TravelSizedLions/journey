using XNode;

using UnityEngine;
using System;

namespace HumanBuilders {

  /// <summary>
  /// A node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("")]
  [Obsolete("Use AnimationParamNode instead.")]
  public class AnimationIntNode : AutoNode {
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
    [SerializeField]
    public Animator Animator;

    /// <summary>
    /// The name of the parameter to set.
    /// </summary>
    [Tooltip("The name of the parameter to set.")]
    [SerializeField]
    public string Parameter;

    /// <summary>
    /// The integer to set in the Animator.
    /// </summary>
    [Tooltip("The integer to set.")]
    [SerializeField]
    public int Value;

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
      
      Animator.SetInteger(Parameter, Value);
    }
  }
}