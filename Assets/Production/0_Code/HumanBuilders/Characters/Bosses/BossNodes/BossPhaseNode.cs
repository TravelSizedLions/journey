
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;
using XNode;

namespace HumanBuilders {

  /// <summary>
  /// A node representing one phase of a boss battle.
  /// </summary>
  [NodeWidth(600)]
  [NodeTint(NodeColors.BOSS_COLOR)]
  [CreateNodeMenu("Bosses/Boss Phase")]
  public class BossPhaseNode : AutoNode {
    //-------------------------------------------------------------------------
    // Input Ports
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The boss that this phase is for. 
    /// </summary>
    [Tooltip("The boss that this phase is for.")]
    [LabelWidth(200)]
    public Boss Boss;

    /// <summary>
    /// Whether or not to start attacking immediately.
    /// </summary>
    [Tooltip("Whether or not to start attacking immediately.")]
    [LabelWidth(200)]
    public bool Attacking = true;

    /// <summary>
    /// The name of the phase.
    /// </summary>
    [Tooltip("The name of the phase.")]
    [LabelText("Name")]
    [LabelWidth(200)]
    public string PhaseName;

    /// <summary>
    /// The number of seconds (roughly) to wait between attacks.
    /// </summary>
    [Tooltip("The number of seconds (roughly) to wait between attacks.")]
    [LabelWidth(200)]
    public float AttackInterval;

    /// <summary>
    /// +/- the attack interval (in seconds)
    /// </summary>
    [Tooltip("+/- the attack interval (in seconds)")]
    [LabelWidth(200)]
    public float AttackIntervalVariance;

    [Space(15, order=0)]

    /// <summary>
    /// The attacks the boss can perform this phase.
    /// </summary>
    [Tooltip("The attacks the boss can perform this phase.")]
    [TableList]
    public List<BossAttack> Attacks;

    [Space(10, order=1)]

    //-------------------------------------------------------------------------
    // Output Ports
    //-------------------------------------------------------------------------
    /// <summary>
    /// A list of dynamic conditions that the node can check to see if it should
    /// transition to a given node.
    /// </summary>
    [Tooltip("A list of dynamic conditions that the node can check to see if it should transition to a given node.")]
    [Output(dynamicPortList=true, connectionType=ConnectionType.Override)]
    [TableList]
    public List<Condition> EndOfPhaseConditions;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      RegisterConditions(EndOfPhaseConditions, "EndOfPhaseConditions");
    }

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (Boss == null) {
        Boss = FindObjectOfType<Boss>();
      }

      Boss.StartPhase(this);

      if (Attacking) {
        Boss.StartAttacking();
      } else {
        Boss.StopAttacking();
      }
    }

    public override void PostHandle(GraphEngine graphEngine) {
      // Do nothing and wait! Overrides default behavior.
    }

    public override IAutoNode GetNextNode() {
      for (int i = 0; i < EndOfPhaseConditions.Count; i++) {
        if (EndOfPhaseConditions[i].ConditionMet()) {
          
        }
      }

      return null;
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