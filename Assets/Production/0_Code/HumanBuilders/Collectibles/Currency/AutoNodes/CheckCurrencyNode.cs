using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

namespace HumanBuilders {
  [NodeWidth(360)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("Currency/Compare Currency")]
  public class CheckCurrencyNode : AutoNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------

    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;


    [PropertyOrder(998)]
    [Space(8)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Success;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Failure;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [Space(8, order=0)]
    [Tooltip("The name of the currency to spend.")]
    [ValueDropdown("Currencies")]
    public string Currency = "-- select a curreny --";

    [Tooltip("The minimum amount needed.")]
    public float Amount;

    private bool succeeded = true;

    //-------------------------------------------------------------------------
    // Dialog Node API
    //-------------------------------------------------------------------------

    public override void Handle(GraphEngine graphEngine) {
      if (IsCurrencySelected() && GameManager.Inventory.GetCurrencyTotal(Currency) >= Amount) {
        succeeded = true;
      } else {
        succeeded = false;
      }
    }

    public override IAutoNode GetNextNode() {
      string name = succeeded ? "Success" : "Failure";
      return (IAutoNode)GetOutputPort(name).Connection.node;
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

    private bool IsCurrencySelected() {
      return Currency != "-- select a curreny --";
    }
  }
}