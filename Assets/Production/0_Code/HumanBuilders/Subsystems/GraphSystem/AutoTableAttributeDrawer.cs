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
      List<AutoTableEntry> entries = new List<AutoTableEntry>();

      string title = string.IsNullOrEmpty(this.Attribute.Title) ? this.Property.NiceName : this.Attribute.Title; 
      SirenixEditorGUI.Title(title, "", TextAlignment.Center, false, true);

      if (SirenixEditorGUI.IconButton(EditorIcons.Plus)) {
        Type entry = typeof(AutoTableEntry<>);
        Type[] args = { this.Attribute.ListItemType };
        Type constructed = entry.MakeGenericType(args);
        entries.Add((AutoTableEntry)Activator.CreateInstance(constructed));
      }

      SirenixEditorGUI.EndBoxHeader();

      for (int i = 0; i < this.ValueEntry.SmartValue.Count; i++) {
        SirenixEditorGUI.BeginBox();
        SirenixEditorGUI.BeginIndentedHorizontal(new GUILayoutOption[] {});
        bool keep = !SirenixEditorGUI.IconButton(EditorIcons.Minus);

        if (keep) {
          SirenixEditorGUI.Title(this.Attribute.ListItemType.Name + " " + i + ": ", "", TextAlignment.Right, false);
          GUILayoutOption[] options = { GUILayout.MinWidth(245) };
          TElement value = (TElement)SirenixEditorFields.PolymorphicObjectField(
            "",
            this.ValueEntry.SmartValue[i],
            this.Attribute.ListItemType,
            false,
            options
          );

          entries.Add(new AutoTableEntry<TElement>(value));
        }
        
        SirenixEditorGUI.EndIndentedHorizontal();
        SirenixEditorGUI.EndBox();
      }

      AutoTable<TElement> table = new AutoTable<TElement>();
      foreach (AutoTableEntry entry in entries) {
        TElement el = (TElement)entry.Value;
        table.Add(el);
      }
      this.ValueEntry.SmartValue = (TList)table;

      if (entries.Count == 0) {
        SirenixEditorGUI.MessageBox("Press \"+\" to add a new " + this.Attribute.ListItemType.Name + ".", MessageType.None, true);
      }
      SirenixEditorGUI.EndBox();
    }
  }
}
#endif
