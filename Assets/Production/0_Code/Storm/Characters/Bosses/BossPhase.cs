
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Storm.Characters.Bosses {
  [Serializable]
  public class BossPhase {

    /// <summary>
    /// The number of seconds (roughly) to wait between attacks.
    /// </summary>
    public float AttackInterval;

    /// <summary>
    /// +/- the attack interval (in seconds)
    /// </summary>
    public float AttackIntervalVariance;

    [TableList]
    public List<BossAttack> Attacks;
  }
}