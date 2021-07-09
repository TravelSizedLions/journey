using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

namespace HumanBuilders {
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
  [Conditional("UNITY_EDITOR")]
  public class ConditionTableAttribute : Attribute {
    public bool StartExpanded;
    public float R, G, B, A;
    public string Title;

    public ConditionTableAttribute(string title, float r, float g, float b) : this(title, r, g, b, 1f) {}
    public ConditionTableAttribute(string title, float r, float g, float b, float a) : this(title, r, g, b, a, false) {}
    public ConditionTableAttribute(string title, float r, float g, float b, bool startExpanded) : this(title, r, g, b, 1f, startExpanded) {}
    public ConditionTableAttribute(string title, float r, float g, float b, float a, bool startExpanded) {
      Title = title;
      R = r;
      G = g;
      B = b;
      A = a;
      StartExpanded = startExpanded;
    }
  }
}