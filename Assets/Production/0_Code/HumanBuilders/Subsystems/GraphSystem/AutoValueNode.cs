using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public abstract class AutoValueNode : AutoNode {
    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Output;

    public abstract object Value { get; }
  }
}