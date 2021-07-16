
using System;
using System.Diagnostics;
using UnityEngine;

namespace HumanBuilders {
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
  [Conditional("UNITY_EDITOR")]
  public class AutoTableAttribute : Attribute {
    public string ColorHex;
    public string Title;

    public Type ListItemType;

    public AutoTableAttribute(Type listItemType) : this (listItemType, "", "#ffffff") {}
    public AutoTableAttribute(Type listItemType, string hex) : this(listItemType, "", hex) {}
    public AutoTableAttribute(Type listItemType, string title, string hex) {
      Title = title;
      ColorHex = hex;
      ListItemType = listItemType;
    }
  }
}