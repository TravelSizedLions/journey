using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Storm.Subsystems.Save;
using UnityEngine;
using XNode;

namespace Storm.Subsystems.Graph {
  /// <summary>
  /// A dialog node for setting a value to 
  /// </summary>
  [NodeWidth(360)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("Dialog/Data/Store Value Node")]
  public class StoreValueNode : AutoNode {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// Where to store this value.
    /// </summary>
    [Tooltip("Where to store this value. This is usually the name of the level.")]
    [ValueDropdown("GetFolders")]
    public string Folder = "-- select a folder --";

    /// <summary>
    /// The name of the custom folder to add the value to.
    /// </summary>
    [ShowIf("UseCustomFolder")]
    [Tooltip("The name of the custom folder to add the value to.")]
    public string CustomFolder;

    [Space(12, order=1)]

    /// <summary>
    /// The name of the value.
    /// </summary>
    [Tooltip("The name of the value.")]
    [ValueDropdown("GetKeys")]
    public string Key = "-- select a key --";

    /// <summary>
    /// The name of the custom key to store the value with.
    /// </summary>
    [Tooltip("The name of the custom key to store the value with.")]
    [ShowIf("UseCustomKey")]
    public string CustomKey;

    [Space(12, order=2)]

    /// <summary>
    /// Whether or not to append a GUID reference to the key. This will
    /// associate the value with a specific object.
    /// </summary>
    [Tooltip("Whether or not to append a GUID reference to the key. This will associate the value with a specific object.")]
    public bool AppendGUID;

    /// <summary>
    /// The GUID to reference.
    /// </summary>
    [Tooltip("The GUID to reference")]
    [ShowIf("AppendGUID", true)]
    public GuidReference GUID;

    [Space(12, order=3)]

    #region Value Fields
    /// <summary>
    /// The type of value to set.
    /// </summary>
    [Tooltip("The type of value to set")]
    [ValueDropdown("types")]
    public string ValueType = "-- select a data type --";

  
    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "bool", true)]
    public bool BoolValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "int", true)]
    public int IntValue;
    
    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "string", true)]
    public string StringValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "double", true)]
    public double DoubleValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "guid", true)]
    public GuidReference GUIDValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "unity/vector2", true)]
    public Vector2 Vec2Value;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "unity/vector3", true)]
    public Vector3 Vec3Value;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "unity/object-position", true)]
    public GuidComponent ObjectPosition;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/byte", true)]
    public byte ByteValue;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/sbyte", true)]
    public sbyte SByteValue;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/short", true)]
    public short ShortValue;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/ushort", true)]
    public ushort UShortValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/uint", true)]
    public uint UIntValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/long", true)]
    public long LongValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/ulong", true)]
    public ulong ULongValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/float", true)]
    public float FloatValue;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/decimal", true)]
    public decimal DecimalValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", "other/char", true)]
    public char CharValue;
    #endregion



    [Space(8, order=4)]

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }

    public override void Handle(GraphEngine graphEngine) {
      bool valid = true;
      if (!IsFolderSelected()) {
        Debug.LogWarning("No folder has been selected from the dropdown!");
        valid = false;
      }

      if (!IsKeySelected()) {
        Debug.LogWarning("No key has been selected from the dropdown!");
        valid = false;
      }

      if (ValueType == "-- select a data type --") {
        Debug.LogWarning("No value has been given to the node to store!");
        valid = false;
      }

      if (!valid) {
        return;
      }

      string folder = UseCustomFolder() ? CustomFolder : Folder;
      string keyPostfix = UseCustomKey() ? CustomKey : Key;
      string guid = AppendGUID ? GUID.ToString() : "";

      string key = guid+keyPostfix;

      dynamic value = GetValueToStore(ValueType);
      VSave.Set(folder, key, value);
    }


    #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector Stuff
    //-------------------------------------------------------------------------
    
    private static IEnumerable types = new List<string> {
      "bool",
      "int",
      "string",
      "double",
      "guid",
      "unity",
      "unity/vector2",
      "unity/vector3",
      "unity/object-position",
      "other",
      "other/byte",
      "other/sbyte",
      "other/short",
      "other/ushort",
      "other/uint",
      "other/long",
      "other/ulong",
      "other/float",
      "other/decimal",
      "other/char",
    };

    public dynamic GetValueToStore(string type) {
      switch (type) {
        case "bool": return BoolValue;
        case "int": return IntValue;
        case "string": return StringValue;
        case "double": return DoubleValue;
        case "guid": return GUID.ToByteArray();
        case "unity/vector2": return new float[] { Vec2Value.x, Vec2Value.y };
        case "unity/vector3": return new float[] { Vec3Value.x, Vec3Value.y };
        case "unity/object-position": return new float[] { GUID.gameObject.transform.position.x, GUID.gameObject.transform.position.y };
        case "other/byte": return ByteValue;
        case "other/sbyte": return SByteValue;
        case "other/short": return ShortValue;
        case "other/ushort": return UShortValue;
        case "other/uint": return UIntValue;
        case "other/long": return LongValue;
        case "other/ulong": return ULongValue;
        case "other/float": return FloatValue;
        case "other/decimal": return DecimalValue;
        case "other/char": return CharValue;
        default: return null;
      }
    }


    /// <summary>
    /// Gets the list of folder names listed in StaticFolders.cs
    /// </summary>
    /// <returns>The list of common folders to save out information to.</returns>
    private IEnumerable GetFolders() {
      FieldInfo[] fields = typeof(StaticFolders).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
      List<string> folders = new List<string>();

      folders.Add("-- select a folder --");

      foreach (FieldInfo field in fields) {
        folders.Add((string)field.GetRawConstantValue());
      }

      folders.Add("other...");

      return folders;
    }

    /// <summary>
    /// Whether or not the custom folder field should display in-editor.
    /// </summary>
    /// <returns>True if the user has selected "other..." from the dropdown
    /// list. False otherwise.</returns>
    private bool UseCustomFolder() {
      return Folder == "other...";
    }

    /// <summary>
    /// Gets the list of keys listed in Keys.cs
    /// </summary>
    /// <returns>The list of keys that information can be paired with.</returns>
    private IEnumerable GetKeys() {
      FieldInfo[] fields = typeof(Keys).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
      List<string> keys = new List<string>();

      keys.Add("-- select a key --");

      foreach (FieldInfo field in fields) {
        keys.Add((string)field.GetRawConstantValue());
      }

      keys.Add("other...");

      return keys;
    }

    /// <summary>
    /// Whether or not the custom key field should display in-editor.
    /// </summary>
    /// <returns>True if the user has selected "other..." from the dropdown
    /// list. False otherwise.</returns>
    private bool UseCustomKey() {
      return Key == "other...";
    }

    /// <summary>
    /// Whether or not a folder has been selected from the drop down.
    /// </summary>
    /// <returns>True if a folder has been selected. False otherwise.</returns>
    private bool IsFolderSelected() {
      return Folder != "-- select a folder --";
    }

    /// <summary>
    /// Whether or not a key has been selected from the drop down.
    /// </summary>
    /// <returns>True if a key has been selected. False otherwise.</returns>
    private bool IsKeySelected() {
      return Key != "-- select a key --";
    }

    private bool IsDataTypeSelect() {
      return ValueType != "-- select a data type --";
    }

    #endregion
  }
}