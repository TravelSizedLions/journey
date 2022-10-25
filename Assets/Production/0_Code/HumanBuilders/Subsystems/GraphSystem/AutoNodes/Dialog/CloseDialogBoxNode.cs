using System.Collections.Generic;
using Sirenix.OdinInspector;


using UnityEngine;
using UnityEngine.EventSystems;
using XNode;

namespace HumanBuilders.Graphing {

  /// <summary>
  /// A node that closes a dialog box UI element.
  /// </summary>
  [NodeTint(NodeColors.DBOX_COLOR)]
  [NodeWidth(400)]
  [CreateNodeMenu("Dialog/Close Dialog Box")]
  public class CloseDialogBoxNode : AutoNode {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]


    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
    #endregion


    #region Dialog Node API
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    /// <summary>
    /// Invoke the events in the list.
    /// </summary>
    public override void Handle(GraphEngine graphEngine) {
      DialogManager.CloseDialogBox();
    }

    #endregion
  }

}