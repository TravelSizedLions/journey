using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [NodeWidth(NodeWidths.SHORT)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Companion/Show or Hide Companion")]
  public class ShowHideCompanionNode : AutoNode {
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
    public bool Show;

    private static Variable companionActive;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (companionActive == null) {
        companionActive = DeveloperSettings.GetSettings().CompanionActiveVariable;
      }

      companionActive.Value = Show;
    }

    private void Reset() {
      name = "Show/Hide Companion";
    }
  }
}