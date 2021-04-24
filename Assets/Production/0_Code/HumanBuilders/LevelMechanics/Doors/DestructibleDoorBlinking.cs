using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A destructible door that blinks.
  /// </summary>
  public class DestructibleDoorBlinking : DestructibleDoor {

    /// <summary>
    /// The collider defined for the eye.
    /// </summary>
    [SerializeField]
    [Tooltip("The collider defined for the eye.")]
    private Collider2D eyeCollider;

    /// <summary>
    /// The amount of time between checking whether or not to blink.
    /// </summary>
    [SerializeField]
    [Tooltip("The amount of time between checking whether or not to blink.")]
    private float blinkTick;

    /// <summary>
    /// How long to hold the blink for.
    /// </summary>
    [SerializeField]
    [Tooltip("How long to hold the blink for.")]
    private float blinkLength;

    /// <summary>
    /// The countdown timer for testing the blink.
    /// </summary>
    private float blinkTickTimer;

    /// <summary>
    /// The countdown timer for holding the blink.
    /// </summary>
    private float blinkHoldTimer;

    /// <summary>
    /// The animator for the blinking eye.
    /// </summary>
    private Animator anim;

    /// <summary>
    /// Shaking component.
    /// </summary>
    private CameraShaker shake;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected new void Awake() {
      base.Awake();
      anim = GetComponent<Animator>();
      shake = GetComponent<CameraShaker>();
    }

    private void Update() {
      if (blinkTickTimer < 0) {
        if (Random.Range(0f, 1f) < 0.1f) {
          anim.SetBool("bool", false);
          blinkHoldTimer = blinkLength;
        }
        blinkTickTimer = blinkTick;
      } else {
        blinkTickTimer -= Time.deltaTime;
      }

      if (blinkHoldTimer > 0) {
        blinkHoldTimer -= Time.deltaTime;
        if (blinkHoldTimer < 0) {
          anim.SetBool("bool", true);
        }
      }
    }

    //-------------------------------------------------------------------------
    // Puzzle API
    //-------------------------------------------------------------------------

    protected override bool IsSolved(object info) {
      bool hitByThrownObject = base.IsSolved(info);
      if (info is Collision2D col) {
        return hitByThrownObject && col.otherCollider == eyeCollider;
      }

      return false;
    }

    protected override void OnSolved() {
      base.OnSolved();
      shake?.Shake();
    }
  }
}