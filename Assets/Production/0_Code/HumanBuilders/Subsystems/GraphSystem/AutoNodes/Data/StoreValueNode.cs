using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.GUID;
using XNode;

namespace HumanBuilders.Graphing {
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Data/Store Data")]
  public class StoreValueNode : AutoNode {

    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Tooltip("The variable to store information into.")]
    public Variable Variable;

    [Space(5)]
    [PropertyOrder(998)]
    [Input(connectionType=ConnectionType.Override)]
    public EmptyConnection Value;

    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    public override void Handle(GraphEngine graphEngine) {
      NodePort inPort = GetInputPort(nameof(Value));
      NodePort outPort = inPort.Connection;
      if (outPort.node is AutoValueNode n) {
        Variable.Value = n.Value;
      }
    }

    public bool IsTypeMatch(Type type, VariableType supportedType) {
      return ((type == typeof(bool) && supportedType == VariableType.Boolean)
        || (type == typeof(float) && supportedType == VariableType.Float)
        || (type == typeof(int) && supportedType == VariableType.Integer)
        || (type == typeof(string) && supportedType == VariableType.String)
        || (type == typeof(GuidReference) && supportedType == VariableType.GUID));
    }

    private IEnumerable<string> Folders() {
      List<string> folders = new List<string>();

      // Gets the list of constant fields that represent all the game's data folders.
      FieldInfo[] fields = typeof(StaticFolders).GetFields(BindingFlags.Public | BindingFlags.Static);
      foreach (FieldInfo field in fields) {
        if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string)) {
          folders.Add((string) field.GetRawConstantValue());
        }
      }

      return folders;
    }
  }
}