using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Subsystems.Graph;

namespace Storm.Characters.Player {
  /// <summary>
  /// A node that unlocks player movement inputs.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Player/Unfreeze Player")]
  public class UnfreezePlayerNode : AutoNode {
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Space(8, order=1)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
    #endregion
    
    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    /// <summary>
    /// Enable movement for the player.
    /// </summary>
    /// <param name="graphEngine">
    /// The graph traversal engine that called into this node.
    /// </param>
    public override void Handle(GraphEngine graphEngine) {
      GameManager.Player?.EnableMove();
      GameManager.Player?.EnableCrouch();
      GameManager.Player?.EnableJump();
    }
    #endregion


  }

}