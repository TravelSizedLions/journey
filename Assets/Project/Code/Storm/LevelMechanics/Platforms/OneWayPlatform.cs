using System;
using System.Collections.Generic;

using Unity;
using UnityEngine;

using Storm.Characters.Player;

namespace Storm.LevelMechanics.Platforms {

  /**
    Platforms that players can hop onto from below.
    Pressing down lets the player through the platform.
    
    Use on a parent object with a composite collider.
    +----------------------+
    | +----+ +----+ +----+ |
    | |    | |    | |    | | 
    | +----+ +----+ +----+ |
    +----------------------+
  */
  public class OneWayPlatform : MonoBehaviour {
  
    private static PlayerCharacter player;
    
    private static BoxCollider2D playerCollider;
    
    /** Platform object > Platform sections*/
    private BoxCollider2D platformCollider;

    private bool droppingThrough;

    private bool playerIsTouching;
    
    private float disableTimer;

    public float disabledTime = 0.5f;

    public void Awake() {
      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
        playerCollider = player.GetComponent<BoxCollider2D>();
      }
      
      platformCollider = GetComponent<BoxCollider2D>();
    }
    
    protected void Update() {
      if (playerIsTouching && Input.GetKeyDown(KeyCode.DownArrow)) {
        platformCollider.enabled = false;
        droppingThrough = true;
        disableTimer = disabledTime;
      }

      if (!platformCollider.enabled) {
        disableTimer -= Time.deltaTime;
      }

      if (disableTimer <= 0) {
        platformCollider.enabled = false;
        droppingThrough = false;
        disableTimer = 0;
      }
    }

    protected void FixedUpdate() {
      // Bottom of player collider.
      float bottomOfPlayerCollider = playerCollider.bounds.center.y - playerCollider.bounds.extents.y;
      
      // Top of platformCollider
      float topOfPlatformCollider = platformCollider.bounds.center.y + platformCollider.size.y/2; 
      
      // The player is rising.
      bool ascending = player.activeMovementMode.rb.velocity.y > 0;

      //TODO: The platform should only be disabled for the player,
      //      which probably means that the platform collider layer needs to change
      //      depending on the test below, with one layer being ignored by
      //      player collisions, instead of just disabling the collider alltogether. 
      //      Switch this over once enemies or dynamic/freebody obstacles become a thing.
      platformCollider.enabled = (bottomOfPlayerCollider >= topOfPlatformCollider) && !(droppingThrough || ascending); 
    
      // Also, MAKE SURE THE ROTATION IS AT ZERO FOR OBJECTS WITH THIS SCRIPT.
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            playerIsTouching = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            playerIsTouching = false;
        }
    }
  }
}