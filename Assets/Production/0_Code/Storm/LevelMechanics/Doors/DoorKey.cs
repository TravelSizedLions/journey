using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using Storm.Subsystems.Reset;
using UnityEngine;

namespace Storm.LevelMechanics.Doors {

  /// <summary>
  /// A key for a locked door. Whenever the key is 
  /// collected, it will signal to the door that it's 
  /// been collected so the door can open.
  /// </summary>
  public class DoorKey : Resetting {

    #region Variables
    #region Debug Info
    [Header("Debug Info", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The door that this key unlocks.
    /// </summary>
    [ReadOnly]
    [Tooltip("The door that this key unlocks.")]
    public LockedDoor Door;

    /// <summary>
    /// Whether or not this key has been collected by the player.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not this key has been collected by the player.")]
    private bool isCollected;
    #endregion

    /// <summary>
    /// A reference to the sprite.
    /// </summary>
    [ReadOnly]
    [Tooltip("A reference to the sprite.")]
    private SpriteRenderer sprite;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    public void Awake() {
      this.sprite = GetComponent<SpriteRenderer>();
    }


    /// <summary>
    /// Collect the key when the player collides with it.
    /// </summary>
    public void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        this.sprite.enabled = false;
        this.isCollected = true;

        GetComponent<Collider2D>().enabled = false;

        this.Door.OnKeyCollected();
      }
    }

    /// <summary>
    /// Make the key re-appear if the player dies before
    /// opening the door.
    /// </summary>
    public override void ResetValues() {
      if (!this.Door.IsOpen()) {
        this.isCollected = false;
        this.sprite.enabled = true;

        GetComponent<Collider2D>().enabled = true;
      }
    }
    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Associate this key with a particular door.
    /// </summary>
    /// <param name="door">The door to associate this key with.</param>
    public void RegisterDoor(LockedDoor door) {
      this.Door = door;
    }

    /// <summary>
    /// Whether or not this key has been collected.
    /// </summary>
    /// <returns>True if this key has already been collected by the player. False otherwise.</returns>
    public bool IsCollected() {
      return isCollected;
    }
    #endregion
  }
}