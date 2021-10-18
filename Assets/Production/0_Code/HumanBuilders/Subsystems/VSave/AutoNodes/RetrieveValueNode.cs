using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Data/Retrieve Data")]
  public class RetrieveValueNode : AutoValueNode {
    public override object Value { get => Variable.Value; }


    [Tooltip("The variable to store information into.")]
    public Variable Variable;
  }
}