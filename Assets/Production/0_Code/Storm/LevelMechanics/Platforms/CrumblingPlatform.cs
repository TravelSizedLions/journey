using System.Collections;
using System.Linq;
using System.Collections.Generic;


using UnityEngine;
using Sirenix.OdinInspector;

namespace HumanBuilders {

  /// <summary>
  /// A platform that's designed to crumble away when the player lands on it.
  /// </summary>
  /// <seealso cref="CrumblingBlock" />
  public class CrumblingPlatform : Resetting {

    #region Variables
    #region Crumbling Settings
    [Header("Crumbling Settings", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// How much of a warning the player gets before the platform disappears completely (in seconds).
    /// </summary>
    /// 
    [Tooltip("How much of a warning the player gets before the platform disappears completely (in seconds).")]
    [SerializeField]
    private float crumblingTime = 0.5f;

    /// <summary>
    /// The amount of time the platform waits before resetting (in seconds).
    /// </summary>
    [Tooltip("The amount of time the platform waits before resetting (in seconds).")]
    [SerializeField]
    private float resetTime = 3.5f;

    /// <summary>
    /// The number of crumblings states attached to each crumbling block in the platform, aside from the initial untouched state. E.x., if there are 4 total states in your animator, set this to 3. 
    /// </summary>
    [Tooltip("The number of crumblings states attached to each crumbling block in the platform, aside from the initial untouched state. E.x., if there are 4 total states in your animator, set this to 3.")]
    [SerializeField]
    private int crumblingStates = 3;

    /// <summary>
    /// Whether or not resetting is enabled for the platform
    /// </summary>
    [Tooltip("Whether or not resetting is enabled for the platform.")]
    [SerializeField]
    private bool canReset = true;

    [Space(10, order=2)]
    #endregion

    #region Timers
    [Header("Timers", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// How much time has passed since the platform has started to crumble. 
    /// </summary>
    [Tooltip("How much time has passed since the platform has started to crumble. ")]
    [SerializeField]
    [ReadOnly]
    private float crumblingTimer;

    /// <summary>
    /// How much time has passed while the platform is waiting to reset.
    /// </summary>
    [Tooltip("How much time has passed while the platform is waiting to reset.")]
    [SerializeField]
    [ReadOnly]
    private float resetTimer;

    [Space(10, order=5)]
    #endregion

    #region State Transition Information
    [Header("State Transition Information", order=6)]
    [Space(5, order=7)]

    /// <summary>
    /// Whether or not the platform is in the process of waiting to reset.
    /// </summary>
    [Tooltip("Whether or not the platform is in the process of waiting to reset.")]
    [SerializeField]
    [ReadOnly]
    private bool waitingToReset;


    /// <summary>
    /// How much each block in the platform has crumbled. This corresponds to which state each crumbling block's animator should be in.
    /// </summary>
    [Tooltip("How much each block in the platform has crumbled. This corresponds to which state each crumbling block's animator should be in.")]
    [SerializeField]
    [ReadOnly]
    private int currentState;

    /// <summary>
    /// The amount of time that should pass between each state of crumbling.
    /// </summary>
    [Tooltip("The amount of time that should pass between each state of crumbling.")]
    [SerializeField]
    [ReadOnly]
    private float timeBetweenStates;

    #endregion

    /// <summary>
    /// The list of crumbling blocks parented to this game object.
    /// </summary>
    private List<CrumblingBlock> blocks;


    /// <summary>
    /// Whether or not the player is overlapping with the area of the platform.
    /// </summary>
    private bool isPlayerOverlapping;

    private CompositeCollider2D compositeCollider;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Start() {
      blocks = new List<CrumblingBlock>(GetComponentsInChildren<CrumblingBlock>());
      crumblingTimer = 0f;
      currentState = 0;
      waitingToReset = false;
      timeBetweenStates = crumblingTime / crumblingStates;

      compositeCollider = GetComponent<CompositeCollider2D>();
    }

    private void Update() {
      // If any of the blocks signal that they should be crumbling away...
      if (blocks.Any(block => block.IsCrumbling)) {
        crumblingTimer += Time.deltaTime;

        if (crumblingTimer > crumblingTime) {
          // Disable the platform temporarily and wait for it to respawn.
          resetTimer = 0;
          waitingToReset = true;
          blocks.ForEach(block => block.Disable());
          blocks.ForEach(block => block.IsCrumbling = false);

        } else if (crumblingTimer > currentState * timeBetweenStates) {
          // Change visible states to indicate to the player how close the 
          // platform is to disappearing.
          currentState++;
          blocks.ForEach(block => block.ChangeState(currentState));
        }
      } else if (canReset && waitingToReset) {
        // wait to reset. 
        resetTimer += Time.deltaTime;

        // Don't reset if the player's in the way.
        if (!isPlayerOverlapping && resetTimer > resetTime) {
          ResetValues();
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        isPlayerOverlapping = true;
      }
    }

    private void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        isPlayerOverlapping = false;
      }
    }

    #endregion

    
    #region Resetting API
    //-------------------------------------------------------------------------
    // Resetting API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Re-enables the platform.
    /// </summary>
    public override void ResetValues() {
      resetTimer = 0;
      crumblingTimer = 0;
      waitingToReset = false;
      currentState = 0;
      blocks.ForEach(block => block.Enable());
      blocks.ForEach(block => block.ChangeState(currentState));
    }

    #endregion

  }

}