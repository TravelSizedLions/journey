using System.Collections;
using System.Collections.Generic;


using Storm.Flexible;
using Storm.Subsystems.Audio;
using UnityEditor;
using UnityEngine;

namespace Storm.Collectibles.Currency {

  /// <summary>
  /// Currency which explodes into smaller chunks of currency and gravitates towards the onscreen wallet UI
  /// when collected by the player.
  /// </summary>
  /// <seealso cref="Currency" />
  /// <seealso cref="Wallet" />
  public class ExplodingCurrency : Currency {

    #region Explosion Variables
    [Space(10, order=0)]
    [Header("Currency Particles", order=1)]
    [Space(5, order=2)]

    /// <summary>
    /// The smaller currency that explodes out of this currency.
    /// </summary>
    [Tooltip("The smaller currency that explodes out of this currency.")]
    public GravitatingCurrency UnitCurrency;

    [Space(8, order=3)]

    /// <summary>
    /// The maximum initial speed at which the currency particles emit from the base piece of currency.
    /// </summary>
    [Tooltip("The maximum initial speed at which the currency particles emit from the base piece of currency.")]
    [SerializeField]
    private float maxParticleVelocity = 80f;


    /// <summary>
    /// The rate at which currency particles decelerate. 0 - Never slow down. 1 - Stop immediately.
    /// </summary>
    [Tooltip("The rate at which currency particles decelerate. 0 - Never slow down. 1 - Stop immediately.")]
    [SerializeField]
    [Range(0,1)]
    private float particleDeceleration = 0.2f;

    /// <summary>
    /// How much deceleration can vary from currency particle to currency particle.
    /// </summary>
    /// <remarks>
    /// If the noise would make the particle deceleration fall out of a reasonable range, then the deceleration is clamped to between [0,1].
    /// </remarks>
    [Tooltip("How much deceleration can vary from currency particle to currency particle. Note - deceleration will never fall out of the range [0,1].")]
    [SerializeField]
    [Range(0,1)]
    private float particleDecelerationNoise = 0.1f;


    [Space(8, order=4)]

    /// <summary>
    /// How slow currency particles need to be going before they begin to gravitate towards the wallet.
    /// </summary>
    [Tooltip("How slow currency particles need to be going before they begin to gravitate towards the wallet.")]
    [SerializeField]
    private float gravitationThreshold = 4.0f;


    /// <summary>
    /// How much the gravitation threshold can vary from currency particle to currency particle.
    /// </summary>
    [Tooltip("How much the gravitation threshold can vary from currency particle to currency particle.")]
    [SerializeField]
    public float gravitationThresholdNoise = 3.5f;


    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();

      particleDeceleration = 1-particleDeceleration;
    }

    #endregion

    #region Collectible API
    //-------------------------------------------------------------------------
    // Collectible API
    //-------------------------------------------------------------------------

    /// <summary>
    /// The particle explodes into several pieces which then begin gravitating towards the 
    /// onscreen wallet.
    /// </summary>
    public override void OnCollected() {
      base.OnCollected();

      // Play a random sound from a pool of sounds for this currency type.
      Sound sound = null;
      foreach (SoundList list in FindObjectsOfType<SoundList>()) {
        if (list.Category.Contains(currencyName)) {
          int soundNum = Random.Range(0, list.Count);
          sound = list[soundNum];
        }
      }

      float unitValue = UnitCurrency.GetValue();
      if (value > unitValue) {
        
        float totalValue = 0;
        int totalCreated = 0;
        while (totalValue < value) {

          
          var currency = Instantiate(UnitCurrency, transform.position, Quaternion.identity);

          var rigibody = currency.GetComponent<Rigidbody2D>();
          if (rigibody != null) {

            rigibody.velocity = new Vector2(
              Random.Range(-maxParticleVelocity, maxParticleVelocity), 
              Random.Range(-maxParticleVelocity, maxParticleVelocity)
            );
          }

          currency.RigidbodyDeceleration = Mathf.Clamp(
            particleDeceleration + Random.Range(-particleDecelerationNoise, particleDecelerationNoise), 0, 1
          );

          currency.GravitationThreshold = Mathf.Clamp(
            gravitationThreshold + Random.Range(-gravitationThresholdNoise, gravitationThresholdNoise), 0, Mathf.Infinity
          );

          currency.OnCollected();

          if (sound != null && totalCreated < 5) {
            AudioManager.Instance.PlayDelayed(sound.Name, totalCreated * soundDelay);
          }

          totalValue += unitValue;
          totalCreated++;
        }
      }

      Destroy(gameObject);
    }

    #endregion
  }
}