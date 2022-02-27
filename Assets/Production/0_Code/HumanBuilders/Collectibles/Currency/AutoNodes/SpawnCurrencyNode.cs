using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  [NodeWidth(360)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Currency/Spawn Currency")]
  public class SpawnCurrencyNode : AutoNode {
    //-------------------------------------------------------------------------
    // I/O Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;


    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    [Space(8)]
    [Tooltip("The spawner to use.")]
    public CurrencySpawner Spawner;

    [HideIf(nameof(Dynamic), false)]
    [Tooltip("The currency to get the current value of.")]
    public float Amount;

    [ShowIf(nameof(Dynamic), false)]
    [Input(connectionType=ConnectionType.Override)]
    [LabelText("Amount")]
    public EmptyConnection DynamicAmount;

    public bool Dynamic;


    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (Spawner != null) {
        if (Dynamic) {
          NodePort inPort = GetInputPort(nameof(DynamicAmount));
          NodePort outPort = inPort.Connection;
          if (outPort.node is AutoValueNode n) {
            Spawner.SetSpawnAmount((float)n.Value);
          } else {
            Debug.LogWarning("Please connect a int or float Value Node to the Amount port.");
          }
        } else {
          Spawner.SetSpawnAmount(Amount);
        }

        Spawner.SpawnCurrency();
      } else {
        Debug.LogWarning("Please add a spawner to your " + nameof(SpawnCurrencyNode) + ".");
      }
    }
  }
}