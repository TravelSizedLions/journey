using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Characters.Bosses;
using UnityEngine;

namespace Storm.Characters.Bosses {
  /// <summary>
  /// A composite class responsible for planning and performing attacks for a boss battle.
  /// </summary>
  /// <seealso cref="BossAttack"/>
  /// <seealso cref="BossPhase" />
  /// <seealso cref="Boss"/>
  public class AttackEngine : MonoBehaviour {


    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The animator for the boss battle.
    /// </summary>
    [SerializeField]
    [Tooltip("The animator for the boss battle.")]
    private Animator bossAnimator = null;

    /// <summary>
    /// The current phase of the boss battle.
    /// </summary>
    [SerializeField]
    [FoldoutGroup("Debug Info")]
    [Tooltip("The current phase of the boss battle.")]
    [ReadOnly]
    private BossPhaseNode currentPhase;

    /// <summary>
    /// The next attack the boss is palnning to perform.
    /// </summary>
    [SerializeField]
    [FoldoutGroup("Debug Info")]
    [Tooltip("The next attack the boss is planning to perform.")]
    [ReadOnly]
    private BossAttack nextAttack;

    /// <summary>
    /// The amount of time left before the next attack, in seconds.
    /// </summary>
    [SerializeField]
    [FoldoutGroup("Debug Info")]
    [Tooltip("The amount of time left before the next attack, in seconds.")]
    [ReadOnly]
    private float nextAttackCountdown;

    /// <summary>
    /// The total frequency of the attacks listed in the phase (ideally they
    /// should always add up to either 1 or 100, but any total works.)
    /// </summary>
    [SerializeField]
    [FoldoutGroup("Debug Info")]
    [Tooltip("The total frequency of the attacks listed in the phase. Ideally this should always add up to either 1 or 100, but any total works.")]
    [ReadOnly]
    private float totalFrequency;

    /// <summary>
    /// Whether or not the boss is planning and performing attacks.
    /// </summary>
    [SerializeField]
    [FoldoutGroup("Debug Info")]
    [Tooltip("Whether or not the boss is planning and performing attacks.")]
    [ReadOnly]
    private bool canAttack;

    /// <summary>
    /// Whether or not the boss is currently performing an attack animation.
    /// </summary>
    [SerializeField]
    [FoldoutGroup("Debug Info")]
    [Tooltip("Whether or not the boss is currently performing an attack animation.")]
    [ReadOnly]
    private bool performingAttack;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Update() {
      if (canAttack) {
        if (nextAttackCountdown > 0) {
          nextAttackCountdown -= Time.deltaTime;
        } else if (!performingAttack) {
          PerformAttack();
        }
      }
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Begin the next phase of the boss battle.
    /// </summary>
    /// <param name="phase">The boss phase</param>
    /// <param name="startAttacking">Whether or not to start planning attacks
    /// immediately (default = true).</param>
    public void StartPhase(BossPhaseNode phase, bool startAttacking = true) {
      currentPhase = phase;
      CalculateFrequency();

      if (startAttacking) {
        StartAttacking();
      }
    }

    /// <summary>
    /// Start planning out and performing attacks.
    /// </summary>
    public void StartAttacking() {
      canAttack = true;
      PlanNextAttack();
    }

    /// <summary>
    /// Stop planning & performing attacks. Also interrupts the current attack.
    /// </summary>
    public void StopAttacking() {
      canAttack = false;
      InterruptAttack();
    }

    /// <summary>
    /// Perform the next attack
    /// </summary>
    public void PerformAttack() {
      bossAnimator.SetTrigger(nextAttack.Trigger);
      performingAttack = true;
    }


    /// <summary>
    /// Immediately stop the current attack if one is being performed.
    /// </summary>
    public void InterruptAttack() {
      bossAnimator.SetTrigger("no_attack");
    }

    /// <summary>
    /// Plan out what and when the next attack will be.
    /// </summary>
    public void PlanNextAttack() {
      performingAttack = false;
      nextAttackCountdown = PickTiming(currentPhase.AttackInterval, currentPhase.AttackIntervalVariance);
      nextAttack = GetNextAttack(currentPhase.Attacks, nextAttack, totalFrequency);
    }

    /// <summary>
    /// Pick the timing of the next attack.
    /// </summary>
    /// <param name="interval">The rough point in the future for this attack.</param>
    /// <param name="variance">a small delta +/- the attack interval.</param>
    /// <returns>The number of seconds before the next attack.</returns>
    public float PickTiming(float interval, float variance) {
      return interval + Random.Range(-variance, variance);
    }

    /// <summary>
    /// Pick an attack from the list.
    /// </summary>
    /// <param name="attacks">The list of attacks to choose from.</param>
    /// <param name="previousAttack">The last attack that was chosen.</param>
    /// <param name="sumFrequency">The sum of all attack frequencies.</param>
    /// <returns>The next attack that should be performed.</returns>
    public BossAttack GetNextAttack(List<BossAttack> attacks, BossAttack previousAttack = null, float sumFrequency = 0) {
      if (sumFrequency == 0) {
        foreach (BossAttack attack in attacks) {
          sumFrequency += attack.Frequency;
        }
      }

      if (attacks.Count > 1) {
        float roll = Random.Range(0, sumFrequency);

        foreach (BossAttack attack in attacks) {
          if (roll < attack.Frequency) {
            if (attack != previousAttack) {
              return attack;
            } else {
              int index = attacks.IndexOf(attack);
              index = PreventDuplicate(index, attacks.Count);
              return attacks[index];
            }
          }

          roll -= attack.Frequency;
        }

        return attacks[attacks.Count - 1];

      } else if (attacks.Count == 1) {
        return attacks[0];
      }

      return null;
    }

    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    private int PreventDuplicate(int index, int totalAttacks) {
      float coinFlip = Random.Range(0f, 1f);
      if (index == 0) {
        index++;
      } else if (index == totalAttacks - 1) {
        index--;
      } else {
        if (coinFlip < 0.5f) {
          index--;
        } else {
          index++;
        }
      }
      return index;
    }


    /// <summary>
    /// Calculates the total frequency of the current phase.
    /// </summary>
    private void CalculateFrequency() {
      totalFrequency = 0;
      foreach (BossAttack attack in currentPhase.Attacks) {
        totalFrequency += attack.Frequency;
      }
    }

    #endregion
  }
}