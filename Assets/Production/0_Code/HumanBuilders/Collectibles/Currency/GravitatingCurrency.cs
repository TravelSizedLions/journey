using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A piece of collectible currency with a specific value. When collected, this currency will
  /// float towards the onscreen wallet UI.
  /// </summary>
  /// <seealso cref="Currency" />
  /// <seealso cref="Wallet" />
  public class GravitatingCurrency : Currency {

    //-------------------------------------------------------------------------
    // Gravitation Settings
    //-------------------------------------------------------------------------
    [Space(10, order=0)]
    [Header("Gravitation Settings", order=1)]
    [Space(5, order=2)]

    /// <summary>
    /// How fast this piece of currency gravitates towards the wallet UI.
    /// </summary>
    [Tooltip("How fast this piece of currency gravitates towards the wallet UI.")]
    [Range(0,1)]
    public float GravitationStrength = 0.25f;

    /// <summary>
    /// The rate at which this currency decelerates. This cancels out rigidbody physics over time. 0 - immediately. 1 - never.
    /// </summary>
    [Tooltip("How quickly rigidbody physics stops affecting gravitation. 0 - Cancel out rigidbody physics immediately. 1 - Do not cancel out rigidbody physics.")]
    [Range(0,1)]
    public float RigidbodyDeceleration = 0.9f;

    /// <summary>
    /// How slow this currency needs to be moving before it begins gravitating towards the wallet.
    /// </summary>
    [HideInInspector]
    public float GravitationThreshold = 0.1f;

    //-------------------------------------------------------------------------
    // Other Variables
    //-------------------------------------------------------------------------
    /// <summary>
    /// A reference to this game object's rigidbody.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Gravitation settings & behavior for this piece of currency.
    /// </summary>
    private Gravitating gravityBehavior;


    /// <summary>
    /// The sprite for this currency.
    /// </summary>
    private SpriteRenderer sprite;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected void Awake() {
      rb = GetComponent<Rigidbody2D>();
      sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
      if (rb != null) {
        rb.velocity *= RigidbodyDeceleration;

        if (gravityBehavior == null && 
            collected && 
            rb.velocity.magnitude < GravitationThreshold) {
          StartGravitating();
        }
      }
    }

    //-------------------------------------------------------------------------
    // Collectible API
    //-------------------------------------------------------------------------
    /// <summary>
    /// When collected, play a sound and begin gravitating towards the onscreen Wallet UI.
    /// </summary>
    public override void OnCollected() {
      if (!collected) {
        base.OnCollected();
        GameManager.Player.AddCurrency(currencyName, Value);


        if (rb == null || rb.velocity.magnitude < GravitationThreshold) {
          StartGravitating();
        }

        // Play a random sound from a pool of sounds for this currency type.
        if (playSounds) {
          PlayRandomSound();
        }

        sprite.sortingLayerName = "UI";
      }
    }

    /// <summary>
    /// Start gravitating towards the wallet UI component.
    /// </summary>
    private void StartGravitating() {
      foreach (Wallet wallet in FindObjectsOfType<Wallet>()) {
        if (wallet.GetCurrencyName() == currencyName) {
          gravityBehavior = gameObject.AddComponent<Gravitating>();
          gravityBehavior.SetGravity(GravitationStrength);
          gravityBehavior.SetRigidbodyDeceleration(RigidbodyDeceleration);
          gravityBehavior.GravitateTowards(wallet.gameObject);
        }
      }
    }

    /// <summary>
    /// Play a random sound from the list of currency collect sounds.
    /// </summary>
    /// <seealso cref="SoundLibrary" />
    private void PlayRandomSound() {
      if (PickupSounds != null) {
        int soundNum = Random.Range(0, PickupSounds.Count);
        Sound s = PickupSounds[soundNum];
        AudioManager.Play(s.Name);
      }
    }
  }
}