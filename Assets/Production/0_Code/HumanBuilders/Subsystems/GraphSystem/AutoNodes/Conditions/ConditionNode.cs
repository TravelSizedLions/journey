
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using HumanBuilders.Attributes;

namespace HumanBuilders.Graphing {
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

    [Space(5)]
    [ShowIf(nameof(HasMultipleConditions))]
    public CombinationLogic Logic = CombinationLogic.AND;

    public override IAutoNode GetNextNode() {
      string portName = PassesConditions() ? "Pass" : "Fail";
      var outputPort = GetOutputPort(portName);
      var inputPort = outputPort.Connection;
      return (IAutoNode)inputPort.node;
    }

    public bool PassesConditions() {
      if (Logic == CombinationLogic.AND) {
        foreach (var cond in Conditions) {
          if (!cond.IsMet()) {
            return false;
          }
        }

        return true;
      } else {
        foreach (var cond in Conditions) {
          if (cond.IsMet()) {
            return true;
          }
        }

        return false;
      }
    }

    private bool HasMultipleConditions() {
      return Conditions?.Count > 1;
    }
  } 
}