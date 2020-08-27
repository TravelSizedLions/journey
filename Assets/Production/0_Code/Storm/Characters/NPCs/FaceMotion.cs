using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.Characters.NPCs {

  /// <summary>
  /// A script that causes the NPC its placed on to always face the direction
  /// it's moving.
  /// </summary>
  public class FaceMotion : MonoBehaviour {

    #region Fields
    /// <summary>
    /// The NPC's animator.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// The position of the NPC in the prior frame.
    /// </summary>
    private Vector3 prevPosition;

    /// <summary>
    /// The way the NPC is currently facing.
    /// </summary>
    private Facing facing;
    #endregion

    #region Unity API
    private void Awake() {
      animator = GetComponent<Animator>();
      prevPosition = transform.position;
    }

    private void FixedUpdate() {
      Vector3 curPosition = transform.position;
      float diff = (curPosition - prevPosition).x;

      if (Mathf.Abs(diff) > 0.1) {
        transform.localScale = Vector3.one;

        float direction = Mathf.Sign(diff);
        if (direction > 0) {
          if (facing != Facing.Right) {
            animator.SetTrigger("run_right");
            facing = Facing.Right;
          }
        } else if (direction < 0) {
          if (facing != Facing.Left) {
            animator.SetTrigger("run_left");
            facing = Facing.Left;
          }
        }
      } else if (facing != Facing.None) {
        if (facing == Facing.Right) {
          transform.localScale = new Vector3(-1, 1, 1);
        }

        animator.SetTrigger("idle");
        facing = Facing.None;
      }

      prevPosition = curPosition; 
    }

    private void OnEnable() {
      transform.localScale = Vector3.one;
    }
    #endregion
  }
}