
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
  [CreateNodeMenu("Bosses/AttackPhase")]
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

    [Space(10, order=0)]

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
    [Output(dynamicPortList=true)]
    [TableList]
    public List<Condition> EndOfPhaseConditions;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Start() {
      RegisterConditions(EndOfPhaseConditions);
    }
    #endregion

    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------

    public override void Handle() {
      
    }

    public override void PostHandle(GraphEngine graphEngine) {

    }
    #endregion
  }
}