using System.Collections.Generic;
using Storm.AudioSystem;
using UnityEngine;

namespace Storm.Collectibles.Currency {

  /// <summary>
  /// A piece of collectible currency with a specific value.
  /// </summary>
  public class Currency : Collectible {

    [SerializeField]
    protected string CurrencyName;


    /** How many "points" this piece of currency is worth */
    [SerializeField]
    protected int value = 1;

    public bool PlaySounds = true;

    protected override void Awake() {
      base.Awake();
    }

    public override void OnCollected() {
      base.OnCollected();
    }

    public string GetName() {
      return CurrencyName;
    }

    public int GetValue() {
      return value;
    }

  }

}