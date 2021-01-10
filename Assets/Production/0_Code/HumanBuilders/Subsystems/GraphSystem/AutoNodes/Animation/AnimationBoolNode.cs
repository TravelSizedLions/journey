using XNode;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Animation Bool")]
  public class AnimationBoolNode : AutoNode {

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
    /// The Animator controller to change. If left blank, the default is the
    /// player character's animator.
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

    #endregion

    #region Unity API
    //---------------------------------------------------
    // Unity API
    //---------------------------------------------------
    private void Awake() {

    }
    #endregion

    #region Auto Node API
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

    #endregion
  }
}