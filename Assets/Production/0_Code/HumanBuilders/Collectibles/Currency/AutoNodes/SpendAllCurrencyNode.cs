using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A node that causes the player to spend some currency.
  /// </summary>
  [NodeWidth(360)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("Currency/Spend All Currency")]
  public class SpendAllCurrencyNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The name of the currency to spend.
    /// </summary>
    [Tooltip("The name of the currency to spend.")]
    [ValueDropdown("Currencies")]
    public string Currency = "-- select a curreny --";

    [Space(8, order=1)]

    /// <summary>
    /// Output connection for the next node given that the player had enough
    /// money to spend.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;



    //-------------------------------------------------------------------------
    // Dialog Node API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Try to spend an amount of currency.
    /// </summary>
    public override void Handle(GraphEngine graphEngine) {
      player.SpendCurrency(Currency, player.GetCurrencyTotal(Currency));
    }


    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------

    private IEnumerable Currencies() {
      List<string> currencies = new List<string>();

      Wallet[] wallets = FindObjectsOfType<Wallet>();
      foreach (Wallet w in wallets) {
        currencies.Add(w.GetCurrencyName());
      }

      return currencies;
    }

    public override bool IsNodeComplete() {
      return base.IsNodeComplete() && Currency != "-- select a curreny --";
    }
  }
}