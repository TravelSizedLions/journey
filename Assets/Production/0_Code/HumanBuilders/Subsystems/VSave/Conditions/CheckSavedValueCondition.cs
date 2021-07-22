using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class CheckSavedValueCondition : AutoCondition {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
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
    // Condition API
    //-------------------------------------------------------------------------
    public override bool IsMet() {
      try {
        switch (this.Type) {
          case SupportedType.Integer:
            return VSave.Get<int>(Folder, Key) == ExpectedInt;
          case SupportedType.Float:
            return VSave.Get<float>(Folder, Key) == ExpectedFloat;
          case SupportedType.String:
            return VSave.Get<string>(Folder, Key) == ExpectedString;
          case SupportedType.Boolean:
            return VSave.Get<bool>(Folder, Key) == ExpectedBool;
          default:
            return false;
        }
      } catch (Exception e) {
        Debug.LogWarning(e.Message);
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