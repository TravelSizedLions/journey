using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Storm.Characters.Bosses {
  public class CreepingRegretAttacks : MonoBehaviour {

    /// <summary>
    /// The animator that has all the animations for the attacks.
    /// </summary>
    public Animator animator;

    [Space(12)]

    /// <summary>
    /// The list of attacks the boss can perform.
    /// </summary>
    public BossPhase PhaseOneAttacks;

    /// <summary>
    /// The second phase of attacks the boss can perform.
    /// </summary>
    public BossPhase PhaseTwoAttacks;

    /// <summary>
    /// The current phase's attack patten.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private BossPhase currentPhase;

    /// <summary>
    /// The time left until the next attack.
    /// </summary>
    private float nextAttackCountdown;

    /// <summary>
    /// Whether or not the boss is currently performing an attack.
    /// </summary>
    private bool performingAttack;

    /// <summary>
    /// Whether or not the boss is allowed to perform attacks
    /// </summary>
    private bool canAttack;

    /// <summary>
    /// The next attack for the boss to perform.
    /// </summary>
    private BossAttack nextAttack;

    /// <summary>
    /// The total frequency of attacks the boss can perform.
    /// </summary>
    private float totalFrequency;

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      animator = gameObject.GetComponent<Animator>();
      ResetPhase();
      canAttack = true;
    }



    private void Start() {
      PlanNextAttack();
    }

    private void Update() {
      if (canAttack) {
        if (nextAttackCountdown > 0) {
          nextAttackCountdown -= Time.deltaTime;
        } else if (!performingAttack) {
          PerformNextAttack();
        }
      }
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Have the boss plan out the next attack based on the frequency table.
    /// </summary>
    public void PlanNextAttack() {
      performingAttack = false;
      nextAttackCountdown = currentPhase.AttackInterval + Random.Range(-currentPhase.AttackIntervalVariance, currentPhase.AttackIntervalVariance);
      
      // Randomly choose an attack based on the frequency table.
      if (currentPhase != null && currentPhase.Attacks.Count > 0) {
        float roll = Random.Range(0, totalFrequency);
        bool found = false;

        foreach (BossAttack attack in currentPhase.Attacks) {
          if (roll < attack.Frequency) {

            if (attack == nextAttack) {
              int index = currentPhase.Attacks.IndexOf(attack);
              index = PreventDuplicate(index);
              nextAttack = currentPhase.Attacks[index];
            } else {
              nextAttack = attack;
            }

            found = true;
            break;
          }

          roll -= attack.Frequency;
        }
        
        if (!found) {
          nextAttack = currentPhase.Attacks[currentPhase.Attacks.Count-1];
        }
      }
    }

    public int PreventDuplicate(int index) {
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
    /// Perform the next attack.
    /// </summary>
    public void PerformNextAttack() {
      animator.SetTrigger(nextAttack.Trigger);
      performingAttack = true;
    }


    /// <summary>
    /// Stop the onslaught of attacks (for now....mwuhahaha...)
    /// </summary>
    public void StopAttacks() {
      canAttack = false;
      InterruptAttack();
    }

    public void InterruptAttack() {
      animator.SetTrigger("no_attack");
    }


    /// <summary>
    /// Start the onslaught of attacks!
    /// </summary>
    public void StartAttacks() {
      canAttack = true;
      PlanNextAttack();
    }


    public void ResetPhase() {
      currentPhase = PhaseOneAttacks;
      CalculateFrequency();
    }


    public void CalculateFrequency() {
      totalFrequency = 0;
      totalFrequency = 0;
      foreach (BossAttack attack in currentPhase.Attacks) {
        totalFrequency += attack.Frequency;
      }
    }

    public void NextPhase() {
      currentPhase = PhaseTwoAttacks;
      CalculateFrequency();
    }

    #endregion
  }
}
