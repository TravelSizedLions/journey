using System.Collections.Generic;


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

namespace HumanBuilders {

  /// <summary>
  /// A node representing list of decisions.
  /// </summary>
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Decision")]
  public class DecisionNode : AutoNode {
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
    public void Decide(int index, GraphEngine graphEngine) {
      prevDecisionIndex = index;
      PostHandle(graphEngine);
    }

    /// <summary>
    /// Set the index of the previous decision at this node.
    /// </summary>
    /// <param name="decisionIndex">The index of the decision made.</param>
    public void SetPreviousDecision(int decisionIndex) {
      prevDecisionIndex = decisionIndex;
    }

    //---------------------------------------------------
    // Auto Node API
    //---------------------------------------------------
    /// <summary>
    /// Ignore normal Dialog Handling. We need to wait for the Player to
    /// actually click on their decision before advancing the dialog.
    /// See <seealso cref="Decide" /> and the class <seealso cref="DecisionBox" />
    /// </summary>
    public override void Handle(GraphEngine graphEngine) {
      prevDecisionIndex = int.MaxValue;
    }

    public override void PostHandle(GraphEngine graphEngine) {
      IAutoNode node = GetNextNode();
      if (node != null) {
        graphEngine.SetCurrentNode(node);
        graphEngine.Continue();
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

    public override bool IsComplete() {
      int inputPorts = 0;
      int outputPorts = 0;
      foreach (NodePort port in Ports) {
        if (port.IsStatic && port.IsOutput) {
          continue;
        }

        if (!port.IsConnected || port.GetConnections().Count == 0) {
          return false;
        }

        inputPorts += port.IsInput ? 1 : 0;
        outputPorts += port.IsOutput ? 1 : 0;
      }

      if (outputPorts == 0) {
        return false;
      }

      return true;
    }

    public override int TotalDisconnectedPorts() {
      int disconnected = 0;
      foreach (NodePort port in Ports) {
        if (port.IsStatic && port.IsOutput) {
          continue;
        }

        if (!port.IsConnected || port.GetConnections().Count == 0) {
          disconnected ++;
        }
      }

      return disconnected;
    }
  }
}
