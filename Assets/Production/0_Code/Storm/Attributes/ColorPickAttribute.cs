using System;

using UnityEngine;
using Sirenix.Utilities;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace Storm.Attributes {
  public class ColorPickerAttribute : Attribute {

  }

  #if UNITY_EDITOR
  public class ColorPickerAttributeDrawer : OdinAttributeDrawer<ColorPickerAttribute, Color> {
    
  }
  #endif
}
