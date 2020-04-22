using System.Collections;
using System.Collections.Generic;
using Storm.AudioSystem;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Collectibles.Currency {

  /// <summary>
  /// A piece of collectible currency with a specific value. When collected, this currency will
  /// float towards the onscreen wallet UI.
  /// </summary>
  /// <seealso cref="Currency" />
  /// <seealso cref="Wallet" />
  public class GravitatingCurrency : Currency {

    #region Variables
    #region Gravitation Settings 
    [Space(10, order=0)]
    [Header("Gravitation Settings", order=1)]
    [Space(5, order=2)]

    /// <summary>
    /// How fast this piece of currency gravitates towards the wallet UI.
    /// </summary>
    [Tooltip("How fast this piece of currency gravitates towards the wallet UI.")]
    [Range(0,1)]
    public float GravitationStrength = 0.125f;

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
    #endregion

    /// <summary>
    /// A reference to this game object's rigidbody.
    /// </summary>
    private new Rigidbody2D rigidbody;

    /// <summary>
    /// Gravitation settings & behavior for this piece of currency.
    /// </summary>
    private Gravitating gravityBehavior;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();

      rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
      if (rigidbody != null) {
        rigidbody.velocity *= RigidbodyDeceleration;

        if (gravityBehavior == null && 
            collected && 
            rigidbody.velocity.magnitude < GravitationThreshold) {
          StartGravitating();
        }
      }
    }

    #endregion

    #region Collectible API
    //-------------------------------------------------------------------------
    // Collectible API
    //-------------------------------------------------------------------------

    /// <summary>
    /// When collected, play a sound and begin gravitating towards the onscreen Wallet UI.
    /// </summary>
    public override void OnCollected() {
      base.OnCollected();

      if (rigidbody == null || rigidbody.velocity.magnitude < GravitationThreshold) {
        StartGravitating();
      }

      // Play a random sound from a pool of sounds for this currency type.
      if (playSounds) {
        PlayRandomSound();
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
    /// <seealso cref="SoundList" />
    private void PlayRandomSound() {
      foreach (SoundList list in FindObjectsOfType<SoundList>()) {
        if (list.Category.Contains(currencyName)) {
          int soundNum = Random.Range(0, list.Count);
          Sound s = list[soundNum];

          AudioManager.Instance.Play(s.Name);
        }
      }
    }

    #endregion
  }
}