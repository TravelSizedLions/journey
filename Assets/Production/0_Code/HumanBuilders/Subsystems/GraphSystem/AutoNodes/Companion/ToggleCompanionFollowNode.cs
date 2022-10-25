using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders.Graphing {
  [NodeWidth(NodeWidths.SHORT)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Companion/Toggle Companion Follow")]
  public class ToggleCompanionFollowNode : AutoNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [Tooltip("Whether or not the player companion is visible")]
    public bool Following;

    private static Variable companionFollowing;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (companionFollowing == null) {
        companionFollowing = DeveloperSettings.GetSettings().CompanionFollowingVariable;
      }

      companionFollowing.Value = Following;
    }
  }
}