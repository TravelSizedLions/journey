using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node representing list of decisions.
  /// </summary>
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Dynamic/Decision Node")]
  public class DecisionNode : AutoNode {

    #region Fields
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(12, order=0)]

    /// <summary>
    /// The list of decisions the player can make.
    /// </summary>
    [Output(dynamicPortList=true)]
    public List<string> Decisions;

    /// <summary>
    /// Whether or not to save the previous decision made at this node.
    /// </summary>
    [Tooltip("Whether or not to save the previous decision made at this node.")]
    public bool SaveDecision;

    /// <summary>
    /// The previous decision made at this node.
    /// </summary>
    private int prevDecisionIndex = 0;
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

    #region Public API
    //---------------------------------------------------
    // Public API
    //---------------------------------------------------
    
    /// <summary>
    /// Get the index of the previous decision made at this node.
    /// </summary>
    public int GetPreviousDecision() {
      return SaveDecision ? prevDecisionIndex : 0;
    }

    /// <summary>
    /// Make the decision and advance the Dialog.
    /// </summary>
    /// <remarks>
    /// This function short-circuits/overrides the normal Dialog handling process.
    /// </remarks>
    /// <param name="index">The index of the decision fromt the list of
    /// decisions presented to the player.</param>
    public void Decide(int index) {
      prevDecisionIndex = index;
      PostHandle();
    }

    /// <summary>
    /// Set the index of the previous decision at this node.
    /// </summary>
    /// <param name="decisionIndex">The index of the decision made.</param>
    public void SetPreviousDecision(int decisionIndex) {
      prevDecisionIndex = decisionIndex;
    }
    #endregion

    #region Dialog Node API
    //---------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------
    
    /// <summary>
    /// Ignore normal Dialog Handling. We need to wait for the Player to
    /// actually click on their decision before advancing the dialog.
    /// See <seealso cref="Decide" /> and the class <seealso cref="DecisionBox" />
    /// </summary>
    public override void Handle() {
      prevDecisionIndex = int.MaxValue;
    }

    public override void PostHandle() {
      IAutoNode node = GetNextNode();
      if (node != null) {
        DialogManager.SetCurrentNode(node);
        DialogManager.ContinueDialog();
      }
    }

    public override IAutoNode GetNextNode() {
      if (prevDecisionIndex < DialogManager.GetDecisionButtons().Count) {
        DialogManager.ClearDecisions();
        NodePort outputPort = GetOutputPort("Decisions "+prevDecisionIndex);
        NodePort inputPort = outputPort.Connection;
        
        return (IAutoNode)inputPort.node;
      } else {
        return null;
      }
    }

    #endregion
  }
}
