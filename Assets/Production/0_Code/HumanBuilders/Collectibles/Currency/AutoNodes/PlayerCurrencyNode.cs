using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [NodeWidth(360)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Currency/Currency Value")]
  public class PlayerCurrencyNode : SmartAutoValueNode<float> {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    public override float SmartValue { get => GameManager.Player.GetCurrencyTotal(Currency); }
    
    [Space(8)]
    [ValueDropdown("Currencies")]
    [SerializeField]
    [Tooltip("The currency to get the current value of.")]
    private string Currency = null;

    private IEnumerable Currencies() {
      List<string> currencies = new List<string>();

      Wallet[] wallets = FindObjectsOfType<Wallet>();
      foreach (Wallet w in wallets) {
        currencies.Add(w.GetCurrencyName());
      }

      return currencies;
    }
  }
}