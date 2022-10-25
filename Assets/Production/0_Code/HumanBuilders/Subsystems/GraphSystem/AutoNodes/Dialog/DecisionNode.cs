using System.Collections.Generic;
using HumanBuilders.Attributes;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

namespace HumanBuilders.Graphing {

  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Decision")]
  public class DecisionNode : AutoNode {
    //---------------------------------------------------
    // Ports
    //---------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// A conditional list of decisions the player can make.
    /// </summary>
    [Space(12, order=0)]
    [PropertyOrder(998)]
    [AutoTable(typeof(ConditionChoice))]
    [Input(dynamicPortList=true)]
    [LabelText("Decisions")]
    [ShowIf(nameof(Dynamic), false)]
    public List<ConditionChoice> DynamicDecisions;

    /// <summary>
    /// The list of decisions the player can make.
    /// </summary>
    [Space(12, order=0)]
    [PropertyOrder(999)]
    [Output(dynamicPortList=true)]
    [HideIf(nameof(Dynamic), false)]
    public List<string> Decisions;

    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// Whether or not to save the previous decision made at this node.
    /// </summary>
    [Tooltip("Whether or not to save the previous decision made at this node.")]
    public bool SaveDecision;

    /// <summary>
    /// The previous decision made at this node.
    /// </summary>
    private int prevDecisionIndex = 0;

    /// <summary>
    /// Whether or not to allow dynamically created decisions.
    /// </summary>
    public bool Dynamic = false;
    
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

    public override bool IsNodeComplete() {
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
