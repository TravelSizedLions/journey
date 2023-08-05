using System.Collections;
using System.Collections.Generic;




using UnityEditor;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// Currency which explodes into smaller chunks of currency and gravitates towards the onscreen wallet UI
  /// when collected by the player.
  /// </summary>
  /// <seealso cref="Currency" />
  /// <seealso cref="Wallet" />
  [RequireComponent(typeof(CurrencySpawner))]
  [RequireComponent(typeof(SelfDestructing))]
  public class ExplodingCurrency : Currency {

    /// <summary>
    /// The mechanism to dispence currency
    /// </summary>
    private CurrencySpawner spawner;

    /// <summary>
    /// Behavior for destroying this game object.
    /// </summary>
    private SelfDestructing destructing;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected void Awake() {
      spawner = GetComponent<CurrencySpawner>();
      destructing = GetComponent<SelfDestructing>();
    }

    //-------------------------------------------------------------------------
    // Collectible API
    //-------------------------------------------------------------------------
    /// <summary>
    /// The particle explodes into several pieces which then begin gravitating towards the 
    /// onscreen wallet.
    /// </summary>
    public override void OnCollected() {
      base.OnCollected();
      if (PickupSounds != null) {
        int soundNum = Random.Range(0, PickupSounds.Count);
        Sound s = PickupSounds[soundNum];
        AlexandriaAudioManager.PlaySound(s);
      }

      spawner.SpawnCurrency();

      StartCoroutine(DestroyAfterFinished());
    }

    private IEnumerator DestroyAfterFinished() {
      while(!spawner.finished) {
        yield return null;
      }

      destructing.KeepDestroyed();
      destructing.SelfDestruct();
    }
  }
}