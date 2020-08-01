using System;
using System.Collections.Generic;
using Storm.Characters.Player;
using Storm.Attributes;
using Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.LevelMechanics.Platforms {

  /// <summary>
  /// Platforms that players can hop onto from below.
  /// Pressing down lets the player through the platform.
  /// </summary>
  public class OneWayPlatform : MonoBehaviour {

    #region Variables

    #region Player Variables
    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private static PlayerCharacter player;

    /// <summary>
    /// A reference to the player's collider.
    /// </summary>
    private BoxCollider2D playerCollider;

    /// <summary>
    /// Whether or not the player is touching this platform.
    /// </summary>
    private bool playerIsTouching;

    /// <summary>
    /// 
    /// </summary>
    private static List<Collider2D> otherColliders;

    #endregion


    #region Temporary Collider Disabling Variables
    /// <summary>
    /// The platform's collider.
    /// </summary>
    private Collider2D platformCollider;


    /// <summary>
    /// A timer to keep the platform's collider disabled for a period.
    /// </summary>
    private float disableTimer;

    /// <summary>
    /// How long to disable the platform's collider when the player is trying to drop through.
    /// </summary>
    [Tooltip("How long to disable the platform's collider when the player is trying to drop through.")]
    public float disabledTime = 0.5f;


    private bool disabled = false;

    /// <summary>
    /// Whether or not the player is trying to drop through the platform.
    /// </summary>
    [Tooltip("Whether or not the player is trying to drop through the platform.")]
    [SerializeField]
    [ReadOnly]
    private bool droppingThrough;

    #endregion
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    public void Awake() {
      platformCollider = GetComponent<BoxCollider2D>();

      PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

      foreach (var p in players) {
        var collider = p.GetComponent<BoxCollider2D>();
        if (collider != null) {
          Physics2D.IgnoreCollision(platformCollider, collider, true);
        }
      }

 
      otherColliders = new List<Collider2D>();
    }

    protected void Update() {
      // Allow the player to drop through.
      if (PlayerShouldDropThrough()) {
        Disable();
        droppingThrough = true;
        disableTimer = disabledTime;
      }

      if (droppingThrough) {
        disableTimer -= Time.deltaTime;
      }

      if (droppingThrough && disableTimer < 0) {
        disableTimer = 0;
        droppingThrough = false;
        disableTimer = 0;
      }
    }
    protected void FixedUpdate() {
      // Check if the player collider should be ignored.
      if (ShouldIgnorePlayerCollider()) {
        Enable();
      } else if (!disabled) {
        Disable();
      }

      // Check each registered collider to see if it should be ignored.
      foreach(Collider2D collider in otherColliders) {
        if (ShouldIgnoreCollider(collider)) {
          Physics2D.IgnoreCollision(platformCollider, collider, true);
        } else {
          Physics2D.IgnoreCollision(platformCollider, collider, false);
        }
      }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player")) {
        playerIsTouching = true;
      }
    }

    private void OnCollisionExit2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player")) {
        playerIsTouching = false;
      }
    }
    #endregion

    
    #region Dealing With the Player

    /// <summary>
    /// Whether or not player should start dropping through the platform.
    /// </summary>
    /// <returns>True if the player should be allowed to drop through the platform. False otherwise.</returns>
    private bool PlayerShouldDropThrough() {
      return playerIsTouching &&
             player.PressedDown() &&
             !player.TryingToMove();
    }

    /// <summary>
    /// Whether or no the player collider should be ignored.
    /// </summary>
    /// <returns>True if the player collider should be ignored. False otherwise.</returns>
    private bool ShouldIgnorePlayerCollider() {
      if (playerCollider == null) {
        player = FindObjectOfType<PlayerCharacter>();
        playerCollider = player.GetComponent<BoxCollider2D>();
      }

      // CHeck if player collider should be ignored.
      float bottomOfPlayerCollider = playerCollider.bounds.center.y - playerCollider.bounds.extents.y;
      float topOfPlatformCollider = platformCollider.bounds.center.y + platformCollider.bounds.extents.y;
      
      return !(droppingThrough) && 
              (bottomOfPlayerCollider >= topOfPlatformCollider);
    }

    /// <summary>
    /// Disable collisions with the player.
    /// </summary>
    private void Disable() {
      disabled = true;
      Physics2D.IgnoreCollision(platformCollider, playerCollider, true);
    }

    /// <summary>
    /// Enable collisions with the player.
    /// </summary>
    private void Enable() {
      disabled = false;
      Physics2D.IgnoreCollision(platformCollider, playerCollider, false);
    }

    #endregion

    #region Dealing With Non-player Objects

    /// <summary>
    /// Add a collider to the list of colliders that can interact with one-way platforms.
    /// </summary>
    /// <param name="collider">The collider.</param>
    public static void RegisterCollider(Collider2D collider) {
      otherColliders.Add(collider);
    }

    /// <summary>
    /// Check whether or not a collider should be ignored by the platform.
    /// </summary>
    /// <param name="collider">The collider to check.</param>
    private bool ShouldIgnoreCollider(Collider2D collider) {
      float bottomOfCollider = collider.bounds.center.y - collider.bounds.extents.y;
      float bottomOfPlatformCollider = platformCollider.bounds.center.y - platformCollider.bounds.extents.y;
      
      return bottomOfCollider < bottomOfPlatformCollider;
    }

    #endregion
  }
}