using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Flexible;
using Storm.AudioSystem;

namespace Storm.Collectibles.Currency {

    /// <summary>
    /// A piece of collectible currency with a specific value.
    /// </summary>
    [RequireComponent(typeof(Gravitating))]
    public class GravitatingCurrency : Currency {


        public float VelocityDecay = 0.5f;

        public float CollectionThreshold = 0.1f;


        private new Rigidbody2D rigidbody;


        protected override void Awake() {
            base.Awake();

            rigidbody = GetComponent<Rigidbody2D>();
        }

        public void FixedUpdate() {
            if (rigidbody != null) {
                rigidbody.velocity *= VelocityDecay;

                if (collected && rigidbody.velocity.magnitude < CollectionThreshold) {
                    StartGravitating();
                }
            }
        }


        public override void OnCollected() {
            base.OnCollected();

            if (rigidbody == null || rigidbody.velocity.magnitude < CollectionThreshold) {
                StartGravitating();
            }

            // Play a random sound from a pool of sounds for this currency type.
            if (PlaySounds) {
                PlayRandomSound();
            }

        }


        private void StartGravitating() {
            foreach (Wallet wallet in FindObjectsOfType<Wallet>()) {
                if (wallet.GetCurrencyName() == CurrencyName) {
                    GetComponent<Gravitating>().SetTarget(wallet.gameObject);
                }
            }
        }


        private void PlayRandomSound() {
            foreach (SoundList list in FindObjectsOfType<SoundList>()) {
                if (list.Category.Contains(CurrencyName)) {
                    int soundNum = Random.Range(0, list.Count);
                    Sound s = list[soundNum];

                    AudioManager.Instance.Play(s.Name);
                }
            }
        }
    }

}