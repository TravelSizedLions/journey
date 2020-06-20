using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;
using Storm.Components;

namespace Storm.Flexible {

  /// <summary>
  /// Something that can be picked up by the player.
  /// </summary>
  public class Carriable : MonoBehaviour {

    #region Fields
    /// <summary>
    /// The name of the indicator that will display over the player's head when
    /// they can interact with this object.
    /// </summary>
    private const string QUESTION_MARK = "QuestionMark";

    private const float SITTING_THRESHOLD = 0.1f;

    /// <summary>
    /// A reference to the player.
    /// </summary>
    private PlayerCharacter player;

    private BoxCollider2D collisionBox;

    public IPhysicsComponent Physics;

    private Vector3 originalScale;
    #endregion

    #region Unity API
    private void Awake() {
      PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
      if (player != null) {
        BoxCollider2D[] cols = GetComponents<BoxCollider2D>();
        collisionBox = cols[0];
        Physics2D.IgnoreCollision(collisionBox, player.GetComponent<BoxCollider2D>());
      }

      Physics = gameObject.AddComponent<PhysicsComponent>();
      originalScale = transform.localScale;
    }

    public void Inject(BoxCollider2D collider) {
      this.collisionBox = collider;
    }

    private void Update() {
      if (player != null) {
        if (player.PressedAction()) {
          player.Pickup(this);
        } else {
          UpdateIndicator(player);
        }
      }
    }

    /// <summary>
    /// Adds or removes the indicator over the player as necessary.
    /// </summary>
    /// <param name="player">A reference to the player.</param>
    private void UpdateIndicator(PlayerCharacter player) {
      if (player.HasIndicator()) {
        if (!ShouldHaveIndicator(player)) {
          player.RemoveIndicator();
        }
      } else {
        if (ShouldHaveIndicator(player)) {
          player.AddIndicator(QUESTION_MARK);
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        player = col.GetComponent<PlayerCharacter>();
      }
    }


    /// <summary>
    /// Removes the indicator over the player. The player will no longer be able
    /// to interact with this object.
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerExit2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        if (player.HasIndicator()) {
          player.RemoveIndicator();
        }
        player = null;
      }
    }

    /// <summary>
    /// Whether or not the player should have an interaction indicator placed
    /// over their head.
    /// </summary>
    /// <param name="player">A reference to the player.</param>
    /// <returns>True if the player should have an interaction indicator over
    /// their head. False otherwise.</returns>
    private bool ShouldHaveIndicator(PlayerCharacter player) {
      return (Mathf.Abs(Physics.Vy) < SITTING_THRESHOLD &&
              !player.IsCrouching() && 
              !player.IsCrawling() && 
              !player.IsDiving());
    }


    public void OnPickup() {
      Physics.Disable();
      Physics.SetParent(player.GetTransform().GetChild(0));
      Physics.ResetLocalPosition();
      collisionBox.enabled = false;
    }

    public void OnPutdown() {
      Physics.Enable();
      Physics.ClearParent();
      collisionBox.enabled = true;
      transform.localScale = originalScale;
    }

    public void OnThrow() {
      Physics.Enable();
      Physics.ClearParent();
      collisionBox.enabled = true;
      transform.localScale = originalScale;
    }

    #endregion
  }
}