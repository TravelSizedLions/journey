using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using UnityEngine;

namespace Storm.LevelMechanics.Doors {

  /// <summary>
  /// A script representing a locked door in a level. Once the player
  /// collects all the keys for the door, they can approach the door
  /// and it will open.
  /// </summary>
  public class LockedDoor : MonoBehaviour {

    #region Variables
    #region Keys
    [Header("Keys", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// The list of key objects needed to open this door.
    /// </summary>
    [Tooltip("The list of key objects needed to open this door.")]
    public List<DoorKey> Keys;

    [Space(15, order = 2)]
    #endregion

    #region Flags
    [Header("Flags", order = 3)]
    [Space(5, order = 4)]

    /// <summary>
    /// Whether or not the door can be opened by the player.
    /// </summary>
    [Tooltip("Whether or not the door can be opened by the player.")]
    [SerializeField]
    [ReadOnly]
    private bool canOpen;


    /// <summary>
    /// Whether or not the door has been opened.
    /// </summary>
    [Tooltip("Whether or not the door has been opened.")]
    [SerializeField]
    [ReadOnly]
    private bool isOpened;
    #endregion

    #region Other Variables
    /// <summary>
    /// The door's collider
    /// </summary>
    private Collider2D[] colliders;


    /// <summary>
    /// The door's sprite
    /// </summary>
    private SpriteRenderer sprite;
    #endregion
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Inject each key for the door with a reference to the door
    /// so that the key can signal when it's been collected.
    /// </summary>
    private void Awake() {
      if (Keys == null) {
        Keys = new List<DoorKey>();
      }

      foreach (var key in Keys) {
        key.RegisterDoor(this);
      }

      colliders = GetComponents<Collider2D>();
      sprite = GetComponent<SpriteRenderer>();
    }


    /// <summary>
    /// Opens the door if all of the keys have been collected.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player") && canOpen) {

        this.isOpened = true;
        this.sprite.enabled = false;

        foreach (var c in this.colliders) {
          c.enabled = false;
        }
      }
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Returns whether or not the do has been opened.
    /// </summary>
    public bool IsOpen() {
      return isOpened;
    }


    /// <summary>
    /// Opens the door
    /// </summary>
    public void OnKeyCollected() {
      Debug.Log("Trying Lock...");
      foreach (var key in Keys) {
        if (!key.IsCollected()) {
          return;
        }
      }
      Debug.Log("Can open!");
      canOpen = true;
    }
    #endregion
  }
}