using XNode;

using UnityEngine;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Dialog/Animation/Animation Bool")]
  public class AnimationBoolNode : DialogNode {


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

    #region 
    private void Awake() {

    }
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
    public override void HandleNode() {
      if (Animator == null) {
        if (player == null) {
          player = GameManager.Instance.player;
        }

        Animator = player.GetComponent<Animator>();
      }

      Animator.SetBool(Parameter, Value);

      if (manager == null) {
        manager = DialogManager.Instance;
      }

      manager.SetCurrentNode(GetNextNode());
      manager.ContinueDialog();
    }

    #endregion
  }
}