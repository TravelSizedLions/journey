using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  public class Blinking : MonoBehaviour {

    /// <summary>
    /// The amount of time between checking whether or not to blink.
    /// </summary>
    [SerializeField]
    [Tooltip("The amount of time between checking whether or not to blink.")]
    private float blinkTick = 0.5f;

    /// <summary>
    /// How long to hold the blink for.
    /// </summary>
    [SerializeField]
    [Tooltip("How long to hold the blink for.")]
    private float blinkLength = 0.25f;

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
    private SpriteRenderer sprite;


    protected void Awake() {
      sprite = GetComponent<SpriteRenderer>();
      sprite.enabled = false;
    }

    private void Update() {
      if (blinkTickTimer < 0) {
        if (Random.Range(0f, 1f) < 0.1f) {
          sprite.enabled = true;
          blinkHoldTimer = blinkLength;
        }
        blinkTickTimer = blinkTick;
      } else {
        blinkTickTimer -= Time.deltaTime;
      }

      if (blinkHoldTimer > 0) {
        blinkHoldTimer -= Time.deltaTime;
        if (blinkHoldTimer < 0) {
          sprite.enabled = false;
        }
      }
    }
  }
}