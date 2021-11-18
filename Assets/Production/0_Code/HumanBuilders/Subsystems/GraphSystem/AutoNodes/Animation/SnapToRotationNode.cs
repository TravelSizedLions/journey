using UnityEngine;
namespace HumanBuilders {

  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Snap To Rotation")]
  public class SnapToRotationNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The Game Object to move.
    /// </summary>
    [Tooltip("The Game Object to move.")]
    public Transform Target;

    /// <summary>
    /// The rotation to set the object to.
    /// </summary>
    [Tooltip("The rotation to set the object to.")]
    public float Rotation;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      // If the target is null, default is to assume it's for the player.
      if (Target == null) {
        Vector3 angles = GameManager.Player.transform.eulerAngles;
        GameManager.Player.transform.localEulerAngles = new Vector3(angles.x, angles.y, Rotation);
      } else {
        Vector3 angles = Target.eulerAngles;
        Target.eulerAngles = new Vector3(angles.x, angles.y, Rotation);
      } 
    }
  }
}