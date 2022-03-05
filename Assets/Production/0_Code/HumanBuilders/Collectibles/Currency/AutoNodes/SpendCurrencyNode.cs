using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;
using XNode;

namespace HumanBuilders {
  /// <summary>
  /// A node that causes the player to spend some currency.
  /// </summary>
  [NodeWidth(360)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("Currency/Spend Currency")]
  public class SpendCurrencyNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    [Tooltip("The name of the currency to spend.")]
    [ValueDropdown("Currencies")]
    public string Currency = "-- select a curreny --";

    [HideIf(nameof(Dynamic), false)]
    [Tooltip("The amount to spend.")]
    public float Amount;

    [ShowIf(nameof(Dynamic), false)]
    [Input(connectionType=ConnectionType.Override)]
    [LabelText("Amount")]
    public EmptyConnection DynamicAmount;

    public bool Dynamic;


    [Space(8, order=1)]

    /// <summary>
    /// Output connection for the next node given that the player had enough
    /// money to spend.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Success;

    /// <summary>
    /// Output connection for the next node given that the player did not have
    /// enough money to spend.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Failure;

    /// <summary>
    /// Whether or not the payment succeeded.
    /// </summary>
    private bool succeeded = true;

    //-------------------------------------------------------------------------
    // Dialog Node API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Try to spend an amount of currency.
    /// </summary>
    public override void Handle(GraphEngine graphEngine) {
      float amount = Dynamic ? GetDynamicAmount() : Amount;
      if (IsCurrencySelected() && GameManager.Inventory.GetCurrencyTotal(Currency) >= amount) {
        GameManager.Inventory.SpendCurrency(Currency, amount);
        succeeded = true;
      } else {
        succeeded = false;
      }
    }

    public override IAutoNode GetNextNode() {
      string name = succeeded ? "Success" : "Failure";
      return (IAutoNode)GetOutputPort(name).Connection.node;
    }

    public float GetDynamicAmount() {
      NodePort inPort = GetInputPort(nameof(DynamicAmount));
      NodePort outPort = inPort.Connection;

      if (outPort.node is AutoValueNode node) {
        Type t = node.Value.GetType();
        if (t != typeof(float) && t != typeof(int)) {
          
          Debug.LogWarning("Dynamic value input should be of type int or float. Actual type: " + t);
          return 0;
        }

        return (float)node.Value;
      }

      Debug.LogWarning("Dynamic value input should be a type of ValueNode.");
      return 0;
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

    public override bool IsNodeComplete() {
      return base.IsNodeComplete() && Currency != "-- select a curreny --";
    }
  }
}