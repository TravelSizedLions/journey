
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Subsystems.Graph;
using UnityEngine;

namespace Storm.Characters.Bosses {
  /// <summary>
  /// A node representing one phase of a boss battle.
  /// </summary>
  [NodeWidth(600)]
  [NodeTint(NodeColors.BOSS_COLOR)]
  [CreateNodeMenu("Bosses/Boss Phase")]
  public class BossPhaseNode : AutoNode {

    #region Input Ports
    //-------------------------------------------------------------------------
    // Input Ports
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]
    #endregion

    #region Fields
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
    #endregion

    #region Output Ports

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
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      RegisterConditions(EndOfPhaseConditions, "EndOfPhaseConditions");
    }
    #endregion

    #region Auto Node API
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
    #endregion
  }
}