using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders.Graphing {

  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.SHORT)]
  [CreateNodeMenu("Companion/Set Companion Interest Point")]
  public class SetCompanionInterestPointNode : AutoNode {

    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8)]
    [PropertyOrder(999)]
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection Output;

    // -------------------------------------------------------------------------
    // Fields
    // -------------------------------------------------------------------------
    [Space(8)]
    [Tooltip("The interest point. The companion will move towards this target.\nIf left empty, the interest point is set to the player.")]
    public Transform target = null;

    // -------------------------------------------------------------------------
    // Auto Node API
    // -------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (GameManager.Companion != null) {
        if (target == null) {
          GameManager.Companion.ClearInterestPoint();
        } else {
          GameManager.Companion.SetInterestPoint(target);
        }
      } else {
        Debug.LogWarning("SetCompanionPosition Node in graph \"" +  graphEngine.GetCurrentGraph().GraphName + "\" is missing a target position. Go into the AutoGraph editor for this graph and find the node with the missing target.");
      }
    }
  }
}