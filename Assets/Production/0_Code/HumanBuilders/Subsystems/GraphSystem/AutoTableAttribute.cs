
using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
  [Conditional("UNITY_EDITOR")]
  public class AutoTableAttribute : TableListAttribute {
    public string ColorHex;
    public string Title;
    public Type ListItemType;

    public bool expanded = false;

    public bool NestObjects = false;

    public AutoTableAttribute(Type listItemType) : this (listItemType, "", "#ffffff") {}
    public AutoTableAttribute(Type listItemType, string hex) : this(listItemType, "", hex) {}
    public AutoTableAttribute(Type listItemType, string title, string hex) : this(listItemType, title, hex, true) {}
    public AutoTableAttribute(Type listItemType, string title, string hex, bool nestObjects) {
      Title = title;
      ColorHex = hex;
      ListItemType = listItemType;
      NestObjects = nestObjects;
    }
  }
}