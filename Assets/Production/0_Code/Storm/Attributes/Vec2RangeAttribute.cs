using System;

using UnityEngine;
using Sirenix.Utilities;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace Storm.Attributes {
  public class Vec2RangeAttribute : Attribute {

    public float minVal;

    public float maxVal;

    public Vec2RangeAttribute(float min, float max) {
      minVal = min;
      maxVal = max; 
    }

  }

  #if UNITY_EDITOR
  public class Vec2RangeAttributeDrawer : OdinAttributeDrawer<Vec2RangeAttribute, Vector2> {
    

    protected override void DrawPropertyLayout(GUIContent label) {
      Rect rect = EditorGUILayout.GetControlRect();

      if (label != null) {
        rect = EditorGUI.PrefixLabel(rect, label);
      }
      

      Vector2  value = this.ValueEntry.SmartValue;
      
      GUIHelper.PushLabelWidth(20);
      value.x = EditorGUI.Slider(rect.AlignLeft(rect.width/2), "X", value.x, this.Attribute.minVal, this.Attribute.maxVal);
      value.y = EditorGUI.Slider(rect.AlignLeft(rect.width/2), "Y", value.x, this.Attribute.minVal, this.Attribute.maxVal);
      GUIHelper.PopLabelWidth();

      this.ValueEntry.SmartValue = value;
    }
  }

  #endif
}