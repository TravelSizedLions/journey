using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Subsystems.Graph;
using UnityEngine;

using XNode;

namespace Storm.Characters.Bosses {


  /// <summary>
  /// The base class for all boss battles in your game.
  /// 
  /// Sub-class from this class in order to create more specific functionality
  /// for your boss battle.
  /// </summary>
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(AutoGraph))]
  [RequireComponent(typeof(AttackEngine))]
  [RequireComponent(typeof(GuidComponent))]
  public class Boss : MonoBehaviour, IBoss {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The total health that the boss has.
    /// </summary>
    public float TotalHealth { get {return totalHealth; } }

    /// <summary>
    /// The total health that the boss has.
    /// </summary>    
    public float RemainingHealth { get { return remainingHealth; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The total health that the boss has.
    /// </summary>
    [Tooltip("The total health that the boss has.")]
    [SerializeField]
    protected float totalHealth = 3;

    /// <summary>
    /// The boss' remaining health.
    /// </summary>
    [Tooltip("The boss' remaining health.")]
    [SerializeField]
    [ReadOnly]
    protected float remainingHealth = 3;
    
    /// <summary>
    /// The phases of this boss fight.
    /// </summary>
    [Tooltip("The graph that represents the phases and cinematic moments of this boss fight.")]
    [LabelText("Battle Graph", true)]
    [SerializeField]
    private AutoGraph graph = null;

    /// <summary>
    /// The graph traversal engine that will run the fight.
    /// </summary>
    private GraphEngine graphEngine;

    /// <summary>
    /// The engine that's responsible for planning and performing attacks for
    /// this boss.
    /// </summary>
    [Tooltip("The engine that's responsible for planning and performing attacks for this boss.")]
    [SerializeField]
    private AttackEngine attackEngine = null;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      graphEngine = gameObject.AddComponent<GraphEngine>();
      graphEngine.StartGraph(graph);
      remainingHealth = totalHealth;
    }

    #endregion


    #region Boss Interface
    //-------------------------------------------------------------------------
    // Boss Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Take a certain amount of damage.
    /// </summary>
    /// <param name="amount">The amount of damage to take.</param>
    public virtual void TakeDamage(float amount) {
      remainingHealth -= amount;
    }


    /// <summary>
    /// Start the next phase of the battle!
    /// </summary>
    /// <param name="bossPhase">The phase to start.</param>
    public virtual void StartPhase(BossPhaseNode bossPhase) {
      attackEngine.StartPhase(bossPhase);
    }

    /// <summary>
    /// Start attacking.
    /// </summary>
    public virtual void StartAttacking() => attackEngine.StartAttacking();

    /// <summary>
    /// Stop attacking. Also interrupts current attack.
    /// </summary>
    public virtual void StopAttacking() => attackEngine.StopAttacking();

    /// <summary>
    /// Reset the boss battle!
    /// </summary>
    public virtual void Reset() {
      remainingHealth = totalHealth;
      graphEngine.StartGraph(graph);
      StopAttacking();
    }
    #endregion

  }
}