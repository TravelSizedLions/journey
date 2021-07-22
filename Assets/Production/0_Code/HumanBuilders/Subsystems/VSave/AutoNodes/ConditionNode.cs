
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

namespace HumanBuilders {
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Conditions/V-Condition")]
  public class ConditionNode : AutoNode {
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [PropertySpace(8)]
    [PropertyOrder(998)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Pass;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Fail;

    [AutoTable(typeof(VCondition), "Conditions", NodeColors.BASIC_COLOR)]
    public List<VCondition> Conditions;

    public override IAutoNode GetNextNode() {
      string portName = PassesConditions() ? "Pass" : "Fail";
      var outputPort = GetOutputPort(portName);
      var inputPort = outputPort.Connection;
      return (IAutoNode)inputPort.node;
    }

    public bool PassesConditions() {
      foreach (var cond in Conditions) {
        if (!cond.IsMet()) {
          return false;
        }
      }

      return true;
    }
  } 
}