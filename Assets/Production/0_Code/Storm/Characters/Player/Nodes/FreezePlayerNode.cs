using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Subsystems.Graph;

namespace Storm.Characters.Player {

  /// <summary>
  /// A node that prevents player character movement.
  /// </summary>
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Player/Freeze Player")]
  public class FreezePlayerNode : AutoNode {

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
    public override void Handle(GraphEngine graphEngine) {
      GameManager.Player.DisableMove();
      GameManager.Player.DisableCrouch();
      GameManager.Player.DisableJump();
    }
    #endregion

  }

}