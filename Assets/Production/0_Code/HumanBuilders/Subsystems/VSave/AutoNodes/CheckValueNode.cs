using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  [NodeWidth(400)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("")]
  [Obsolete("Untested and already deprecated, do not use.")]
  public class CheckValueNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(12, order=0)]

    [ValueDropdown("GetStaticFolders")]
    [FoldoutGroup("Location")]
    public string Folder;

    /// <summary>
    /// The name of the value to check.
    /// </summary>
    [Tooltip("The name of the value to check.")]
    [FoldoutGroup("Location", true)]
    public string Key;



    //-------------------------------------------------------------------------
    // Supported Types
    //-------------------------------------------------------------------------
    public enum SupportedType { Integer, Float, String, Boolean }

    /// <summary>
    /// The type of the value to check.
    /// </summary>
    [FoldoutGroup("Value", true)]
    [Tooltip("The type of the value to check.")]
    public SupportedType Type;

    [ShowIf("Type", SupportedType.Integer)]
    [FoldoutGroup("Value", true)]
    public int ExpectedInt;

    [ShowIf("Type", SupportedType.Boolean)]
    [FoldoutGroup("Value", true)]
    public bool ExpectedBool;

    [ShowIf("Type", SupportedType.Float)]
    [FoldoutGroup("Value", true)]
    public float ExpectedFloat;

    [ShowIf("Type", SupportedType.String)]
    [FoldoutGroup("Value", true)]
    public string ExpectedString;

    //-------------------------------------------------------------------------
    // Outputs
    //-------------------------------------------------------------------------
    [Space(8, order=2)]

    /// <summary>
    /// The output connection if the condition is met.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Pass;

    /// <summary>
    /// The output connection if the condition fails.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Fail;

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override IAutoNode GetNextNode() {
      string portName = IsExpectedValue() ? "Pass" : "Fail";
      NodePort outputPort = GetOutputPort(portName);
      NodePort inputPort = outputPort.Connection;
      return (IAutoNode)inputPort.node;
    }

    public bool IsExpectedValue() {
      string folder = StaticFolders.QUEST_DATA;
      try {
        switch (this.Type) {
          case SupportedType.Integer:
            return VSave.Get<int>(folder, Key) == ExpectedInt;
          case SupportedType.Float:
            return VSave.Get<float>(folder, Key) == ExpectedFloat;
          case SupportedType.String:
            return VSave.Get<string>(folder, Key) == ExpectedString;
          case SupportedType.Boolean:
            return VSave.Get<bool>(folder, Key) == ExpectedBool;
          default:
            return false;
        }
      } catch (Exception) {
        return false;
      }
    }

    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
    private IEnumerable<string> GetStaticFolders() {
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