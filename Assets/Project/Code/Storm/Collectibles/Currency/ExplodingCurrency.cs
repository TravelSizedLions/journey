using System.Collections;
using System.Collections.Generic;
using Storm.AudioSystem;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Collectibles.Currency {

  public class ExplodingCurrency : Currency {


    #region Explosion
    [Space(10, order=0)]
    [Header("Explosion", order=1)]
    [Space(5, order=2)]


    public GravitatingCurrency UnitCurrency;

    [Space(8, order=3)]

    public float explosionVelocity = 80f;

    public float velocityDecay = 0.8f;

    public float velocityDecayNoise = 0.1f;


    [Space(8, order=4)]
    public float gravityThreshold = 4.0f;

    public float gravityThresholdNoise = 3.5f;

    #endregion


    protected override void Awake() {
      base.Awake();
    }

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

      if (value > 1) {
        for (int i = 0; i < value; i++) {
          var currency = Instantiate(UnitCurrency, transform.position, Quaternion.identity);

          var rigibody = currency.GetComponent<Rigidbody2D>();
          if (rigibody != null) {

            rigibody.velocity = new Vector2(Random.Range(-explosionVelocity, explosionVelocity), Random.Range(-explosionVelocity, explosionVelocity));
          }

          currency.VelocityDecay = velocityDecay + Random.Range(-velocityDecayNoise, velocityDecayNoise);
          currency.CollectionThreshold = gravityThreshold + Random.Range(-gravityThresholdNoise, gravityThresholdNoise);

          currency.OnCollected();

          if (sound != null && i < 5) {
            AudioManager.Instance.PlayDelayed(sound.Name, i * soundDelay);
          }

        }
      }

      Destroy(gameObject);
    }
  }
}