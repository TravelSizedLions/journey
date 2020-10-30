using Sirenix.OdinInspector;
using Storm.Characters.Bosses;
using UnityEngine;

namespace Storm.Characters.Bosses {
  /// <summary>
  /// A composite class responsible for planning and performing attacks for a boss battle.
  /// </summary>
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
      float variance = Random.Range(-currentPhase.AttackIntervalVariance, currentPhase.AttackIntervalVariance);
      nextAttackCountdown = currentPhase.AttackInterval + variance;
    
      // Randomly choose an attack based on the frequency table.
      if (currentPhase != null && currentPhase.Attacks.Count > 0) {
        float roll = Random.Range(0, totalFrequency);
        bool found = false;

        foreach (BossAttack attack in currentPhase.Attacks) {
          if (roll < attack.Frequency) {

            
            if (attack != nextAttack) {
              nextAttack = attack;
            } else {
              // Don't perform the same attack twice in a row.
              int index = currentPhase.Attacks.IndexOf(attack);
              index = PreventDuplicate(index);
              nextAttack = currentPhase.Attacks[index];
            }

            found = true;
            break;
          }

          roll -= attack.Frequency;
        }

        // Just to cover our bases, if the frequency check fails, just attack
        // with the last attack in the list.
        if (!found) {
          nextAttack = currentPhase.Attacks[currentPhase.Attacks.Count-1];
        }
      }
    }

    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    private int PreventDuplicate(int index) {
      float coinFlip = Random.Range(0f, 1f);
      if (index == 0) {
        index++;
      } else if (index == currentPhase.Attacks.Count - 1) {
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