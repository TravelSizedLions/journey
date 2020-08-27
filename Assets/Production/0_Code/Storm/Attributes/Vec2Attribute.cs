using System;

using UnityEngine;
using Sirenix.Utilities;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace Storm.Attributes {
  public class Vec2Attribute : Attribute {
    public float x;
    public float y;

  }

  #if UNITY_EDITOR
  public class Vec2AttributeDrawer : OdinAttributeDrawer<Vec2Attribute, Vector2> {
    

    protected override void DrawPropertyLayout(GUIContent label) {
      Rect rect = EditorGUILayout.GetControlRect();

      if (label != null) {
        rect = EditorGUI.PrefixLabel(rect, label);
      }
      
      Vector2  value = this.ValueEntry.SmartValue;
      
      GUIHelper.PushLabelWidth(20);
      value.x = EditorGUI.FloatField(rect.AlignLeft(rect.width/2), this.Attribute.x);
      value.y = EditorGUI.FloatField(rect.AlignLeft(rect.width/2), this.Attribute.y);
      GUIHelper.PopLabelWidth();

      this.ValueEntry.SmartValue = value;
    }
  }

  #endif
}