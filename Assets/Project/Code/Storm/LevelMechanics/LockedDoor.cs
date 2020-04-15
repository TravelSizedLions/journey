using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using UnityEngine;

namespace Storm.LevelMechanics {

  /// <summary>
  /// A script representing a locked door in a level. Once the player
  /// collects all the keys for the door, they can approach the door
  /// and it will open.
  /// </summary>
  public class LockedDoor : MonoBehaviour {
    // Start is called before the first frame update

    #region Keys
    [Header("Keys", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// The list of key objects needed to open this door.
    /// </summary>
    [Tooltip("The list of key objects needed to open this door.")]
    public List<DoorKey> keys;

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


    /// <summary>
    /// The door's collider
    /// </summary>
    private Collider2D[] colliders;


    /// <summary>
    /// The door's sprite
    /// </summary>
    private SpriteRenderer sprite;


    /// <summary>
    /// Inject each key for the door with a reference to the door
    /// so that the key can signal when it's been collected.
    /// </summary>
    void Awake() {
      if (keys == null) {
        keys = new List<DoorKey>();
      }

      foreach (var key in keys) {
        key.door = this;
      }

      colliders = GetComponents<Collider2D>();
      sprite = GetComponent<SpriteRenderer>();
    }


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
      foreach (var key in keys) {
        if (!key.isCollected) {
          return;
        }
      }
      Debug.Log("Can open!");
      canOpen = true;
    }


    /// <summary>
    /// Opens the door if all of the keys have been collected.
    /// </summary>
    public void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player") && canOpen) {
        Debug.Log("Opening!");
        this.isOpened = true;
        this.sprite.enabled = false;

        foreach (var c in this.colliders) {
          c.enabled = false;
        }
      }
    }
  }
}