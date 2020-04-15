using System.Collections;
using System.Collections.Generic;
using Storm.AudioSystem;
using Storm.Flexible;
using UnityEngine;

namespace Storm.Collectibles.Currency {

  public class ExplodingCurrency : Currency {

    public GravitatingCurrency UnitCurrency;

    public float Explosiveness;

    public float VelocityDecay;

    public float VelocityDecayNoise;

    public float CollectionThreshold;

    public float CollectionThresholdNoise;

    public float soundDelay;

    protected override void Awake() {
      base.Awake();
    }

    public override void OnCollected() {
      base.OnCollected();

      // Play a random sound from a pool of sounds for this currency type.
      Sound sound = null;
      foreach (SoundList list in FindObjectsOfType<SoundList>()) {
        if (list.Category.Contains(CurrencyName)) {
          int soundNum = Random.Range(0, list.Count);
          sound = list[soundNum];
        }
      }

      if (value > 1) {
        for (int i = 0; i < value; i++) {
          var currency = Instantiate(UnitCurrency, transform.position, Quaternion.identity);

          var rigibody = currency.GetComponent<Rigidbody2D>();
          if (rigibody != null) {

            rigibody.velocity = new Vector2(Random.Range(-Explosiveness, Explosiveness), Random.Range(-Explosiveness, Explosiveness));
          }

          currency.VelocityDecay = VelocityDecay + Random.Range(-VelocityDecayNoise, VelocityDecayNoise);
          currency.CollectionThreshold = CollectionThreshold + Random.Range(-CollectionThresholdNoise, CollectionThresholdNoise);

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