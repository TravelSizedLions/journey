#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders {
  public class AutoTableAttributeDrawer<TList, TElement> : OdinAttributeDrawer<AutoTableAttribute, TList>
    where TList : AutoTable<TElement> {

    protected override void DrawPropertyLayout(GUIContent label) {
      if (ColorUtility.TryParseHtmlString(this.Attribute.ColorHex, out Color color)) {
        GUIHelper.PushColor(color, true);
      } else {
        GUIHelper.PushColor(Color.white, true);
      }

      SirenixEditorGUI.BeginBox();
      SirenixEditorGUI.BeginBoxHeader();
      GUIHelper.PopColor();

      string title = string.IsNullOrEmpty(this.Attribute.Title) ? this.Property.NiceName : this.Attribute.Title; 
      SirenixEditorGUI.Title(title, "", TextAlignment.Left, false, false);

      if (SirenixEditorGUI.IconButton(EditorIcons.Plus)) {
        Type entry = typeof(AutoTableEntry<>);
        Type[] args = { this.Attribute.ListItemType };
        Type constructed = entry.MakeGenericType(args);
        this.ValueEntry.SmartValue.Add((AutoTableEntry)Activator.CreateInstance(constructed));
      }

      SirenixEditorGUI.EndBoxHeader();

      // While loop used since it's possible to modify the list mid iteration.
      int i = 0;
      while (i < this.ValueEntry.SmartValue.Count) {
        SirenixEditorGUI.BeginBox();
        SirenixEditorGUI.BeginIndentedHorizontal(new GUILayoutOption[] {});
        bool keep = !SirenixEditorGUI.IconButton(EditorIcons.Minus);

        if (keep) {
          SirenixEditorGUI.Title(this.Attribute.ListItemType.Name.Split('`')[0] + " " + i + ": ", "", TextAlignment.Right, false);
          GUILayoutOption[] options = { GUILayout.MinWidth(245) };
          this.ValueEntry.SmartValue[i] = (TElement)SirenixEditorFields.PolymorphicObjectField(
            "",
            this.ValueEntry.SmartValue[i],
            this.Attribute.ListItemType,
            false,
            options
          );

          i++;
        } else {
          this.ValueEntry.SmartValue.RemoveAt(i);
        }
        
        SirenixEditorGUI.EndIndentedHorizontal();
        SirenixEditorGUI.EndBox();
      }

      if (this.ValueEntry.SmartValue.Count == 0) {
        SirenixEditorGUI.MessageBox("Press \"+\" to add a new " + this.Attribute.ListItemType.Name.Split('`')[0] + ".", MessageType.None, true);
      }
      SirenixEditorGUI.EndBox();
    }
  }
}
#endif
