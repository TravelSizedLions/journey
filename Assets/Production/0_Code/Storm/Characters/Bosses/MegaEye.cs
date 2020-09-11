using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using Storm.Subsystems.Reset;
using UnityEngine;

namespace Storm.Characters.Bosses {
  /// <summary>
  /// The largest eye for the boss "creeping regret."
  /// </summary>
  [RequireComponent(typeof(Shaking))]
  public class MegaEye : TriggerableParent, IResettable {
    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------

    /// <summary>
    /// Whether or no the eye is open.
    /// </summary>
    public bool IsOpen { get {return open; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The number of times the main eye needs to be hit.
    /// </summary>
    [Tooltip("The number of times the main eye needs to be hit.")]
    public int TotalHealth;

    /// <summary>
    /// The number of mini eyes to open at the beginning of the battle.
    /// </summary>
    [Tooltip("The number of mini eyes to open at the beginning of the battle.")]
    public int NumEyesStart;

    /// <summary>
    /// The number of eyes that get added onto the open mini eyes after each
    /// attack on the main eye.
    /// </summary>
    [Tooltip("The number of eyes that get added onto the open mini eyes after each attack on the main eye.")]
    public int NumEyesAdded;

    /// <summary>
    /// The number of eyes that will be opened this round.
    /// </summary>
    [Tooltip("The number of eyes that will be opened this round.")]
    [SerializeField]
    [ReadOnly]
    private int numEyes;

    /// <summary>
    /// The remaining number of times the main eye needs to be hit.
    /// </summary>
    [Tooltip("The remaining number of times the main eye needs to be hit.")]
    [SerializeField]
    [ReadOnly]
    private int remainingHealth;
    
    /// <summary>
    /// The list of miniature eyes connected to the main eye.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private MiniEye[] miniEyes;

    /// <summary>
    /// A reference to the animator on this object.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// A component for shaking the eye.
    /// </summary>
    private Shaking shaking;


    /// <summary>
    /// Whether or not the eye is open.
    /// </summary>
    private bool open;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      animator = GetComponent<Animator>();
      shaking = GetComponent<Shaking>();
      miniEyes = FindObjectsOfType<MiniEye>();
      remainingHealth = TotalHealth;
      numEyes = NumEyesStart;
    }

    private void Start() {
      foreach (MiniEye eye in miniEyes) {
        eye.OnEyeClose += TryOpen;
      }
      
      Reset();
    }
    #endregion

    #region Triggerable Parent API
    /// <summary>
    /// If the main eye is open & is hit with a carriable object, the boss takes
    /// damage
    /// </summary>
    /// <param name="col">The collider that hit the boss's main eye.</param>
    public override void PullTriggerEnter2D(Collider2D col) {
      Carriable carriable = col.GetComponent<Carriable>();
      if (carriable != null && col == carriable.Collider && open) {
        TakeDamage();
        numEyes += NumEyesAdded;
        OpenRandomEyes();
        carriable.Physics.Velocity = Vector2.zero;
      }
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Open the eye.
    /// </summary>
    public void Open() {
      animator.SetBool("open", true);
      open = true;
    }

    /// <summary>
    /// Close the eye.
    /// </summary>
    public void Close() {
      animator.SetBool("open", false);
      open = false;
    }

    /// <summary>
    /// Close all mini eyes in the scene.
    /// </summary>
    public void CloseMiniEyes() {
      foreach (MiniEye eye in miniEyes) {
        eye.Close();
      }
    }

    /// <summary>
    /// Reset the boss.
    /// </summary>
    public void Reset() {
      if (animator == null) {
        animator = GetComponent<Animator>();
      }

      remainingHealth = TotalHealth;
      numEyes = NumEyesStart;

      Close();
      CloseMiniEyes();
      OpenRandomEyes();
    }
    #endregion


    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Functions
    //-------------------------------------------------------------------------

    /// <summary>
    /// Try to open the main eye. If there are any small eyes still open, this
    /// does nothing.
    /// </summary>
    private void TryOpen() {
      if (AnyEyesOpen()) {
        return;
      }

      Open();
    }

    /// <summary>
    /// Check to see if any small eyes are open.
    /// </summary>
    /// <returns>True if at least one small eye is open. False otherwise.</returns>
    private bool AnyEyesOpen() {
      foreach (MiniEye eye in miniEyes) {
        if (eye.IsOpen) {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Check to see if all small eyes are open.
    /// </summary>
    /// <returns>True if all small eyes are open. False otherwise.</returns>
    private bool AllEyesOpen()  {
      foreach (MiniEye eye in miniEyes) {
        if (!eye.IsOpen) {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Make the boss take damage. If the boss hits 0 health, the boss will be destroyed.
    /// </summary>
    private void TakeDamage() {
      remainingHealth--;

      if (remainingHealth == 0) {
        foreach (MiniEye eye in miniEyes) {
          Destroy(eye.gameObject);
        }

        Destroy(gameObject);
      } else {
        animator.SetTrigger("damage");
        shaking.Shake();
        Close();
      }
    }

    /// <summary>
    /// Open a random set of small eyes. If the boss is at 1 health, opens all
    /// small eyes.
    /// </summary>
    private void OpenRandomEyes() {      
      if (numEyes > miniEyes.Length || AllEyesOpen()) {
        return;
      } else if (remainingHealth == 1) {
        foreach (MiniEye eye in miniEyes) {
          eye.Open();
        }
        return;
      }

      for (int i = 0; i < numEyes; i++) {
        bool opened = false;
        
        // Open a random eye. If the selected eye is already open,
        // choose a different one.
        while (!opened) {
          int index = Random.Range(0, miniEyes.Length);
          if (!miniEyes[index].IsOpen) {
            miniEyes[index].Open();
            opened = true; 
          }
        }

      }
    }
    
    #endregion
  }
}

