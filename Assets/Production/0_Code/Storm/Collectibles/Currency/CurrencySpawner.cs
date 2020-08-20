using System.Collections;
using Sirenix.OdinInspector;
using Storm.Subsystems.Audio;
using UnityEngine;
// using Storm.Subsystems.Audio;

namespace Storm.Collectibles.Currency {
  public class CurrencySpawner : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The smaller currency that explodes out of this currency.
    /// </summary>
    [Tooltip("The smaller currency that explodes out of this currency.")]
    public GravitatingCurrency UnitCurrency;

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


    /// <summary>
    /// How slow currency particles need to be going before they begin to gravitate towards the wallet.
    /// </summary>
    [Tooltip("How slow currency particles need to be going before they begin to gravitate towards the wallet.")]
    [SerializeField]
    private float gravThreshold = 4.0f;


    /// <summary>
    /// How much the gravitation threshold can vary from currency particle to currency particle.
    /// </summary>
    [Tooltip("How much the gravitation threshold can vary from currency particle to currency particle.")]
    [SerializeField]
    public float gravThresholdNoise = 3.5f;


    /// <summary>
    /// How much currency to spawn, in terms of value. If the unit currency's
    /// value is 5, and this is set to 25, then 5 units will be spawn.
    /// </summary>
    [Tooltip("How much currency to spawn, in terms of value.\n\nIf the unit currency's value is 5, and this is set to 25, then 5 units will be spawn.")]
    [SerializeField]
    private float amountToSpawn = 5;

    /// <summary>
    /// How much time to spawn all currency in seconds.
    /// </summary>
    [Tooltip("How much time to spawn all currency in seconds.")]
    [SerializeField]
    [Range(0, 0.5f)]
    private float spawnSpeed = 0.2f;

    /// <summary>
    /// The time between spawning a piece of currency, in seconds.
    /// </summary>
    private float spawnTimer;

    /// <summary>
    /// Whether or not the spawner has finished spawning currency.
    /// </summary>
    [ReadOnly]
    public bool finished;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      particleDeceleration = 1-particleDeceleration;
      spawnTimer = (spawnSpeed/amountToSpawn);
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    public void SpawnCurrency() {
      StartCoroutine(Spawn());
    }

    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    private IEnumerator Spawn() {
      finished = false;

      // Play a random sound from a pool of sounds for this currency type.
      Sound sound = null;
      foreach (SoundList list in FindObjectsOfType<SoundList>()) {
        if (list.Category.Contains(UnitCurrency.GetName())) {
          int soundNum = Random.Range(0, list.Count);
          sound = list[soundNum];
        }
      }

      float unitValue = UnitCurrency.GetValue();
      if (amountToSpawn> unitValue) {
        
        float totalValue = 0;
        int totalCreated = 0;
        while (totalValue < amountToSpawn) {
                    
          yield return new WaitForSeconds(spawnTimer);

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
            gravThreshold + Random.Range(-gravThresholdNoise, gravThresholdNoise), 0, Mathf.Infinity
          );

          currency.OnCollected();

          if (sound != null && totalCreated < 5) {
            AudioManager.Instance.PlayDelayed(sound.Name, totalCreated * UnitCurrency.GetSoundDelay());
          }

          totalValue += unitValue;
          totalCreated++;
        }
      }

      finished = true;

    }

    #endregion
  }
}