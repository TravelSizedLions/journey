using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Transitions;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// A node that ends the graph by loading a cutscene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("End/Load Cutscene")]
  public class StartCutsceneNode : AutoNode {
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous nodes(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The name of the cutscene to play.
    /// </summary>
    public string Cutscene;
    #endregion

    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      TransitionManager.MakeTransition(Cutscene);
    }

    public override void PostHandle(GraphEngine graphEngine) {
      DialogManager.EndDialog();
      DialogManager.SetCurrentNode(null);
    }
    #endregion
  }
}