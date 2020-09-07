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
  [RequireComponent(typeof(CurrencySpawner))]
  [RequireComponent(typeof(SelfDestructing))]
  public class ExplodingCurrency : Currency {

    #region Explosion Variables
    CurrencySpawner spawner;

    /// <summary>
    /// Unused. Instead, set "Amount to Spawn" on the CurrencySpawner.
    /// </summary>
    [Tooltip("Unused in this case. Instead, set \"Amount to Spawn\" on the CurrencySpawner.")]
    public new float value;


    /// <summary>
    /// Behavior for destroying this game object.
    /// </summary>
    private SelfDestructing destructing;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();

      spawner = GetComponent<CurrencySpawner>();
      destructing = GetComponent<SelfDestructing>();
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

    #endregion
  }
}