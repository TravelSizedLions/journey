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
    protected float totalHealth = 3;

    /// <summary>
    /// The boss' remaining health.
    /// </summary>
    [Tooltip("The boss' remaining health.")]
    protected float remainingHealth = 3;
    
    /// <summary>
    /// The phases of this boss fight.
    /// </summary>
    [Tooltip("The phases of this boss fight.")]
    public AutoGraph graph;

    /// <summary>
    /// The graph traversal engine that will run the fight.
    /// </summary>
    private GraphEngine graphEngine;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      graphEngine = gameObject.AddComponent<GraphEngine>();
      graphEngine.StartGraph(graph);
    }

    #endregion


    #region Boss Interface
    //-------------------------------------------------------------------------
    // Boss Interface
    //-------------------------------------------------------------------------
    public virtual void TakeDamage(float amount) {
      remainingHealth -= amount;
    }
    #endregion

  }
}