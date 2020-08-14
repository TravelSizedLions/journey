
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node which causes a delay in the conversation.
  /// </summary>
  [NodeTint("#996e39")]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Dynamic/Delay Node")]
  public class DelayNode : DialogNode {

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
    
    public override void HandleNode() {
      if (manager == null) {
        manager = DialogManager.Instance;
      }

      manager.StartCoroutine(Wait());
    }

    private IEnumerator Wait() {
      yield return new WaitForSeconds(Seconds);

      manager.SetCurrentNode(GetNextNode());
      manager.ContinueDialog();
    }
    #endregion
  }
}
