using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Storm.Subsystems.Dialog {
  /// <summary>
  /// A dialog node for setting a value to 
  /// </summary>
  [NodeWidth(360)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("Dialog/Data/Store Value Node")]
  public class StoreValueNode : DialogNode {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// Where to store this value. This is usually the name of the level.
    /// </summary>
    [Tooltip("Where to store this value. This is usually the name of the level.")]
    public string Folder;

    /// <summary>
    /// The name of the value.
    /// </summary>
    [Tooltip("The name of the value.")]
    public string Key;

    [Space(12, order=2)]

    #region Value Fields
    /// <summary>
    /// The type of value to set.
    /// </summary>
    [Tooltip("The type of value to set")]
    [ValueDropdown("types")]
    public int ValueType;

  
    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 0, true)]
    public bool BoolValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 1, true)]
    public int IntValue;
    
    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 2, true)]
    public string StringValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 3, true)]
    public double DoubleValue;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 4, true)]
    public GuidComponent GUIDValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 6, true)]
    public Vector2 Vec2Value;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 7, true)]
    public Vector3 Vec3Value;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 9, true)]
    public byte ByteValue;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 10, true)]
    public sbyte SByteValue;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 11, true)]
    public short ShortValue;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 12, true)]
    public ushort UShortValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 13, true)]
    public uint UIntValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 14, true)]
    public long LongValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 15, true)]
    public ulong ULongValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 16, true)]
    public float FloatValue;


    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 17, true)]
    public decimal DecimalValue;

    /// <summary>
    /// The value to store.
    /// </summary>
    [Tooltip("The value to store.")]
    [ShowIf("ValueType", 18, true)]
    public char CharValue;
    #endregion

    [Space(12, order=3)]

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

    
    public override void Handle() {
      
    }

    public override void PostHandle() {
      // Do nothing and wait for the next input.
    }

    #region Casting...
    //-------------------------------------------------------------------------
    // Casting Values
    //-------------------------------------------------------------------------
    
    private static IEnumerable types = new ValueDropdownList<int> {
      {"bool",             0},
      {"int",              1},
      {"string",           2},
      {"double",           3},
      {"guid",             4},
      {"unity",            5},
      {"unity/vector2",    6},
      {"unity/vector3",    7},
      {"other",            8},
      {"other/byte",       9},
      {"other/sbyte",      10},
      {"other/short",      11},
      {"other/ushort",     12},
      {"other/uint",       13},
      {"other/long",       14},
      {"other/ulong",      15},
      {"other/float",      16},
      {"other/decimal",    17},
      {"other/char",       18},
    };


    private static Dictionary<int,Type> intToType = new Dictionary<int, Type>() {
      {0, typeof(bool)},
      {1, typeof(int)},
      {2, typeof(string)},
      {3, typeof(double)},
      {4, typeof(Guid)},
      {6, typeof(Vector2)},
      {7, typeof(Vector3)},
      {9, typeof(byte)},
      {10, typeof(sbyte)},
      {11, typeof(short)},
      {12, typeof(ushort)},
      {13, typeof(uint)},
      {14, typeof(long)},
      {15, typeof(ulong)},
      {16, typeof(float)},
      {17, typeof(decimal)},
      {18, typeof(char)},
    };

    private static Dictionary<Type,int> typeToInt = new Dictionary<Type,int>() {
      {typeof(bool), 0},
      {typeof(int), 1},
      {typeof(string), 2},
      {typeof(double), 3},
      {typeof(Guid), 4},
      {typeof(Vector2), 6},
      {typeof(Vector3), 7},
      {typeof(byte), 9},
      {typeof(sbyte), 10},
      {typeof(short), 11},
      {typeof(ushort), 12},
      {typeof(uint), 13},
      {typeof(long), 14},
      {typeof(ulong), 15},
      {typeof(float), 16},
      {typeof(decimal), 17},
      {typeof(char), 18},
    };


    public object GetValue(int type) {
      Type valueType = null;

      switch (type) {
        case 0: return BoolValue;
        case 1: return IntValue;
        case 2: return StringValue;
        case 3: return DoubleValue;
        case 4: return GUID;
        case 6: return Vec2Value;
        case 7: return Vec3Value;
        case 9: return ByteValue;
        case 10: return SByteValue;
        case 11: return ShortValue;
        case 12: return UShortValue;
        case 13: return UIntValue;
        case 14: return LongValue;
        case 15: return ULongValue;
        case 16: return FloatValue;
        case 17: return DecimalValue;
        case 18: return CharValue;
        default:
          break;
      }

      return valueType;
    }


    // private bool IsTypeSelected(object type) {
    //   return ;
    // }

    #endregion
  }
}