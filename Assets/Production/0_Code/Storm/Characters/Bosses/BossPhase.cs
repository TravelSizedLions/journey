
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Storm.Characters.Bosses {
  /// <summary>
  /// One attack phase of a boss. Each phase contains a list of attacks the boss
  /// can perform, and settings for how quickly the boss can perform them to
  /// control the tempo of the battle.
  /// </summary>
  [Serializable]
  public class BossPhase {

    /// <summary>
    /// The name of the phase.
    /// </summary>
    [Tooltip("The name of the phase.")]
    [FoldoutGroup("Phase")]
    [LabelText("Name")]
    public string PhaseName;

    /// <summary>
    /// The number of seconds (roughly) to wait between attacks.
    /// </summary>
    [Tooltip("The number of seconds (roughly) to wait between attacks.")]
    [FoldoutGroup("Phase")]
    public float AttackInterval;

    /// <summary>
    /// +/- the attack interval (in seconds)
    /// </summary>
    [Tooltip("+/- the attack interval (in seconds)")]
    [FoldoutGroup("Phase")]
    public float AttackIntervalVariance;

    /// <summary>
    /// The attacks the boss can perform this phase.
    /// </summary>
    [Tooltip("The attacks the boss can perform this phase.")]
    [TableList]
    [FoldoutGroup("Phase")]
    public List<BossAttack> Attacks;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">The name of the boss phase.</param>
    /// <param name="attackInterval">The number of seconds (roughly) to wait
    /// between attacks.</param>
    /// <param name="variance">+/- the attack interval (in seconds).</param>
    /// <param name="attacks">The list of attacks that belong to this boss phase.</param>
    public BossPhase(string name, float attackInterval, float variance, List<BossAttack> attacks = null) {
      PhaseName = name;
      AttackInterval = attackInterval;
      AttackIntervalVariance = variance;
      
      if (attacks == null) {
        Attacks = new List<BossAttack>();
      } else {
        Attacks = attacks;
      }
    }

  }
}