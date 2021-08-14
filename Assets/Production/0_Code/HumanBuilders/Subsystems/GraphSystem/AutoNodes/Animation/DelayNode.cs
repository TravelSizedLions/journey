
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using XNode;

namespace HumanBuilders {

  /// <summary>
  /// A node which causes a delay.
  /// </summary>
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.SHORT)]
  [CreateNodeMenu("Animation/Delay")]
  public class DelayNode : AutoNode {

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
    /// The number of seconds to wait.
    /// </summary>
    [Tooltip("The number of seconds to wait.")]
    public float Seconds;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;
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
    public override object GetValue(NodePort port) {
      return null;
    }
    #endregion

    #region Dialog Node API
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    /// <summary>
    /// Waits a predetermined number of seconds before playing the next node in
    /// the conversation.
    /// </summary>
    public override void Handle(GraphEngine graphEngine) {
      graphEngine.StartThread(Wait(graphEngine));
    }

    /// <summary>
    /// Waits the predetermined number of seconds before playing the next node.
    /// </summary>
    private IEnumerator Wait(GraphEngine graphEngine) {
      if (graphEngine.LockNode()) {
        yield return new WaitForSeconds(Seconds);
        graphEngine.UnlockNode();        
      }
    }
    #endregion
  }
}
